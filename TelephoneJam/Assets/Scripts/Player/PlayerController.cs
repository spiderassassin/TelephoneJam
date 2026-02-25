using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    public Camera camera;
    public CharacterController cc;
    public GameObject root;
    public float maxFloatHeight = 6f;
    public float minFloatHeight = 6f;
    public Animator animator;
    public float currentHeight = 100f;

    private float _velocity; public float GetVelocity() { return _velocity; }

    [SerializeField]
    private float _maxSpeed = 15f;
    [SerializeField]
    private float _accel = 36f;
    [SerializeField]
    private float _nearbyBoostTime = 3f;
    [SerializeField]
    private ParticleSystem[] _wingTipParticles;
    [SerializeField]
    private GameObject _dashParticleSource;
    private ParticleSystem[] _dashParticles = new ParticleSystem[8];
    private float[] _lastBoostHitTimes = new float[8];

    private Vector2 _frameTurnInput;
    private Vector2 _facing;
    private Vector2 _visualTurn;
    private float _smoothedMoveUp;
    private float _yawInputForVisuals;
    private float _yawInputForPassiveCamera;
    private float _forwardVisualPitch;
    private float _forwardVisualRoll;
    private float _forwardVisualBlend;

    [SerializeField]
    private float _yawKeyboardInputScale = 1f;
    [SerializeField]
    private float _pitchTurnRate = 120f;
    [SerializeField]
    private float _yawTurnRate = 120f;
    [SerializeField]
    private float _rollVisualInputScale = 15f;
    [SerializeField]
    private float _climbSpeed = 6f;
    [SerializeField]
    private float _descendSpeed = 6f;
    [SerializeField]
    private float _climbResponse = 6f;
    [SerializeField]
    private float _descendResponse = 6f;
    [SerializeField]
    private float _forwardClimbPitchMax = 18f;
    [SerializeField]
    private float _forwardDescendPitchMax = 18f;
    [SerializeField]
    private float _forwardBankMax = 22f;
    [SerializeField]
    private float _forwardVisualResponse = 8f;
    [SerializeField]
    private float _forwardVisualBlendResponse = 8f;

    [SerializeField]
    private AudioSource flying;

    

    private float _flyDownBoost = 1.0f;



    private Vector3 MoveVector => new Vector3(
        (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
        (Input.GetKey(KeyCode.Space) ? 1 : 0)  - (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ? 1 : 0),
        (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        currentHeight = transform.position.y;
        cc = root.GetComponent<CharacterController>();
        for(int i = 0; i < 8; i++)
        {
            var gObj = Instantiate(_dashParticleSource);
            gObj.transform.parent = transform;
            gObj.SetActive(true);
            gObj.transform.localPosition = Quaternion.AngleAxis(i * 45f, Vector3.forward) * new Vector3(-0.75f, 0f) + new Vector3(0, 1.25f);
            gObj.transform.localRotation = Quaternion.AngleAxis(i * 45f, Vector3.forward);
            _dashParticles[i] = gObj.GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isFlying = Mathf.Abs(_velocity) > 1f || MoveVector.z != 0;
        if(isFlying){
            if(!flying.isPlaying){
                flying.Play();
            }
        }
        else
        {
            flying.Stop();
        }

        if(!GameManager.Instance.playerPaused)
        {
            Fly();
        }
        else
        {
            DontFly();
        }
        animator.SetFloat("flyingBlend", _velocity / (_maxSpeed / 2f)); 
        MainCamera.Instance.SetIsFlying(isFlying);
        MainCamera.Instance.PlayerFlightInput(_smoothedMoveUp, _yawInputForPassiveCamera);
        MainCamera.Instance.PlayerTurnInput(_yawInputForPassiveCamera * _rollVisualInputScale);

        HandleTipParticles(_velocity > 1f);
        HandleBoost();
    }

    private void HandleBoost()
    {
        var layer = LayerMask.GetMask("Environment");
        var hasSufficientVelocity = _velocity > _maxSpeed / 4f;
        for(int i = 0; i < 8; i++)
        {
            if(hasSufficientVelocity && 
                Physics.Raycast(transform.position + new Vector3(0, 1.25f), Quaternion.AngleAxis(i * 45f, Vector3.forward) * new Vector3(-1, 0f), 4f, layer))
            {
                _lastBoostHitTimes[i] = Time.time;
                _dashParticles[i].Play();
            }
            else
            {
                if(Time.time - _lastBoostHitTimes[i] >= _nearbyBoostTime || !hasSufficientVelocity)
                {
                    _dashParticles[i].Stop();
                }
            }
        }
    }

    private void HandleTipParticles(bool isFlying)
    {
        if(_wingTipParticles.Length == 0) return;
        bool particleOn = _wingTipParticles[0].isEmitting;
        if(isFlying != particleOn)
        {
            foreach(var particles in _wingTipParticles)
            {
                if(isFlying) particles.Play();
                else particles.Stop();
            }
        }
    }

    public void TryTurn(float pitchDelta, float yawDelta)
    {
        AddTurnInput(pitchDelta, yawDelta);
    }

    public void AddTurnInput(float pitchInput, float yawInput)
    {
        _frameTurnInput += new Vector2(pitchInput, yawInput);
    }

    private void CaptureKeyboardTurnInput(float moveRight)
    {
        AddTurnInput(0f, moveRight * _yawKeyboardInputScale);
    }

    private Vector2 ConsumeTurnInputStep()
    {
        Vector2 clampedInput = new Vector2(
            Mathf.Clamp(_frameTurnInput.x, -1f, 1f),
            Mathf.Clamp(_frameTurnInput.y, -1f, 1f));
        _yawInputForVisuals = clampedInput.y;
        float pitchStep = clampedInput.x * _pitchTurnRate * Time.deltaTime;
        float yawStep = clampedInput.y * _yawTurnRate * Time.deltaTime;
        return new Vector2(pitchStep, yawStep);
    }

    private void Fly(){
        var moveVector = MoveVector;

        var moveForwards = moveVector.z;

        var moveRight = moveVector.x;
        var moveUp = moveVector.y;
        float verticalResponse = moveUp >= 0f ? _climbResponse : _descendResponse;
        _smoothedMoveUp = Mathf.Lerp(_smoothedMoveUp, moveUp, Time.deltaTime * verticalResponse);

        CaptureKeyboardTurnInput(moveRight);
        Vector2 turnStep = ConsumeTurnInputStep();
        _yawInputForPassiveCamera = moveForwards > 0f ? _yawInputForVisuals : 0f;

        float takeX = turnStep.x;
        float takeY = turnStep.y;

        // process turning
        _facing = new Vector2(Mathf.Clamp(_facing.x + takeX, -89, 89), _facing.y + takeY);
        root.transform.rotation = Quaternion.AngleAxis(_facing.y, Vector3.up) * Quaternion.AngleAxis(_facing.x, Vector3.right);

        // process FX
        bool useForwardVisual = moveForwards > 0f;
        float pitchScale = _smoothedMoveUp >= 0f ? _forwardClimbPitchMax : _forwardDescendPitchMax;
        float pitchTarget = -Mathf.Clamp(_smoothedMoveUp, -1f, 1f) * pitchScale;
        float rollTarget = Mathf.Clamp(-_yawInputForVisuals, -1f, 1f) * _forwardBankMax;
        _forwardVisualPitch = Mathf.Lerp(_forwardVisualPitch, useForwardVisual ? pitchTarget : 0f, Time.deltaTime * _forwardVisualResponse);
        _forwardVisualRoll = Mathf.Lerp(_forwardVisualRoll, useForwardVisual ? rollTarget : 0f, Time.deltaTime * _forwardVisualResponse);

        if (!useForwardVisual)
        {
            _visualTurn += new Vector2(takeX, takeY);
        }
        Vector2 nonForwardTurnForDisplay = _visualTurn;
        _visualTurn = Vector2.Lerp(_visualTurn, Vector2.zero, Time.deltaTime * 12f);

        Vector3 nonForwardVisualEuler = new Vector3(
            Mathf.Clamp(nonForwardTurnForDisplay.x, -30, 30),
            Mathf.Clamp(nonForwardTurnForDisplay.y, -30, 30),
            Mathf.Clamp(-nonForwardTurnForDisplay.y / 4f, -30, 30));
        Vector3 forwardVisualEuler = new Vector3(_forwardVisualPitch, 0f, _forwardVisualRoll);
        float targetForwardBlend = useForwardVisual ? 1f : 0f;
        _forwardVisualBlend = Mathf.Lerp(_forwardVisualBlend, targetForwardBlend, Time.deltaTime * _forwardVisualBlendResponse);
        transform.localRotation = Quaternion.Slerp(
            Quaternion.Euler(nonForwardVisualEuler),
            Quaternion.Euler(forwardVisualEuler),
            _forwardVisualBlend);

        bool isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var maxSpeed = _velocity < 0 ? _maxSpeed / 4 : (isBoosting ? _maxSpeed * 2f : _maxSpeed);


        if(moveForwards != 0)
        {
            var oldSpeed = Mathf.Abs(_velocity);
            _velocity += _accel * (isBoosting ? 2f : 1f) * Time.deltaTime * moveForwards;
            if(Mathf.Abs(_velocity) > maxSpeed)
            {
                if(oldSpeed < maxSpeed)
                {
                    _velocity = maxSpeed * Mathf.Sign(_velocity);
                }
                else
                {
                    _velocity = oldSpeed * Mathf.Sign(_velocity);
                }
            }
        }
        else
        {
            _velocity = Mathf.Lerp(_velocity, 0, Time.deltaTime * 2f);

        }

        var downBoost = 1.0f + Mathf.Max(-root.transform.forward.y, 0.0f) / 2f;
        if(downBoost > _flyDownBoost) _flyDownBoost = Mathf.Lerp(_flyDownBoost, downBoost, Time.deltaTime * 10f);
        else _flyDownBoost = Mathf.Lerp(_flyDownBoost, downBoost, Time.deltaTime);

        var count = 0f;
        foreach(var time in _lastBoostHitTimes)
        {
            if(Time.time - time <= _nearbyBoostTime)
            {
                count++;
            }
        }
        var nearbyBoost = 1.0f + 1f / _lastBoostHitTimes.Length / 2f * count;

        float verticalSpeed = _smoothedMoveUp >= 0f ? _climbSpeed : _descendSpeed;
        cc.Move(_velocity * _flyDownBoost * nearbyBoost * Time.deltaTime * root.transform.forward 
            + root.transform.rotation * new Vector3(0, _smoothedMoveUp * verticalSpeed, 0) * Time.deltaTime);

        _frameTurnInput = Vector2.zero;
    }

    private void DontFly(){
        _frameTurnInput = Vector2.zero;
        _yawInputForVisuals = 0f;
        _yawInputForPassiveCamera = 0f;
        _smoothedMoveUp = 0f;
        _forwardVisualPitch = 0f;
        _forwardVisualRoll = 0f;
        _forwardVisualBlend = 0f;
        _visualTurn = Vector2.zero;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
