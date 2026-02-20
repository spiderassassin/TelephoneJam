using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEditor.Rendering;
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

    [SerializeField]
    private float _maxTurnAmount = 60f;
    private Vector2 _wishTurnVector = new Vector2();

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

    private Vector2 _turnSmoothing;
    private Vector2 _facing;
    private Vector2 _visualTurn;
    private float _smoothedMoveUp;

    private float _flyDownBoost = 1.0f;



    private Vector3 MoveVector => new Vector3(
        (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
        (Input.GetKey(KeyCode.Space) ? 1 : 0)  - (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ? 1 : 0),
        (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));


    public void WishTurn(float rightLeft, float upDown)
    {
        _wishTurnVector += new Vector2(rightLeft, upDown);
        _wishTurnVector = Vector2.ClampMagnitude(_wishTurnVector, 60f);
    }

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
        MainCamera.Instance.PlayerTurnInput(_turnSmoothing.y);

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

    public void TryTurn(float dx, float dy)
    {
        _turnSmoothing += new Vector2(dx, dy);
        _turnSmoothing = new Vector2(Mathf.Clamp(_turnSmoothing.x, -90, 90), Mathf.Clamp(_turnSmoothing.y, -90, 90));
    }

    private void Fly(){
        var moveVector = MoveVector;

        var moveForwards = moveVector.z;

        var moveRight = moveVector.x;
        var moveUp = moveVector.y;
        _smoothedMoveUp = Mathf.Lerp(_smoothedMoveUp, moveUp, Time.deltaTime * 6f);

        TryTurn(0, moveRight * Time.deltaTime * 120f);


        // Vector3 velocityForward = _velocity.normalized;
        // if(velocityForward.sqrMagnitude == 0) velocityForward = wishDir;
        // if(velocityForward.sqrMagnitude == 0) velocityForward = dirForwards;

        // transform.rotation = Quaternion.LookRotation(velocityForward);

        var takeX = Mathf.Clamp(_turnSmoothing.x / 40f, -Time.deltaTime * 600f, Time.deltaTime * 600f);
        var takeY = Mathf.Clamp(_turnSmoothing.y / 40f, -Time.deltaTime * 600f, Time.deltaTime * 600f);
        _turnSmoothing -= new Vector2(takeX, takeY);

        // process turning
        _facing = new Vector2(Mathf.Clamp(_facing.x + takeX, -89, 89), _facing.y + takeY);
        root.transform.rotation = Quaternion.AngleAxis(_facing.y, Vector3.up) * Quaternion.AngleAxis(_facing.x, Vector3.right);

        // process FX
        _visualTurn += new Vector2(takeX, takeY);
        transform.localEulerAngles = new Vector3(Mathf.Clamp(_visualTurn.x, -30, 30), Mathf.Clamp(_visualTurn.y, -30, 30), Mathf.Clamp(-_visualTurn.y / 4f, -30, 30));
        _visualTurn =  Vector2.Lerp(_visualTurn, Vector2.zero, Time.deltaTime * 12f);

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

        cc.Move(_velocity * _flyDownBoost * nearbyBoost * Time.deltaTime * root.transform.forward 
            + root.transform.rotation * new Vector3(0, _smoothedMoveUp * 6f, 0) * Time.deltaTime);
    }

    private void DontFly(){
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
