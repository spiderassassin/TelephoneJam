using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public static MainCamera Instance => _instance;
    private static MainCamera _instance;


    [SerializeField]
    private GameObject _cameraSpace;
    [SerializeField]
    private GameObject _cameraOffset;
    [SerializeField]
    private float _minZoom = 3f;
    [SerializeField]
    private float _maxZoom = 12f;
    [SerializeField]
    private float _minAngle = -89;
    [SerializeField]
    private float _maxAngle = 89f;
    [SerializeField]
    private float _maxYawOffset = 80f;
    [SerializeField]
    private float _zoomSpeed = 1f;
    [SerializeField]
    private float _doubleClickTime = 0.25f;
    [SerializeField]
    private float _neutralReturnSpeed = 6f;
    [SerializeField]
    private float _passiveClimbPitchOffset = 8f;
    [SerializeField]
    private float _passiveDescendPitchOffset = 8f;
    [SerializeField]
    private float _passiveYawLeadOffset = 12f;
    [SerializeField]
    private float _passiveYawPanOffset = 0.75f;
    [SerializeField]
    private float _passiveYawPanResponse = 4f;
    [SerializeField]
    private float _passiveYawLeadResponse = 4f;

    private float _doubleClickTimer = 0;
    private bool _isMouseLook = false; public void SetMouseLook(bool value) { _isMouseLook = value; }
    private float _wantedZoom;
    private Vector3 _cameraVector; // x / y = xy rotation in degrees

    private float _initialSpaceHeight;
    private float _initialOffsetHeight;
    private float _spaceOffsetCorrection;
    private bool _isFlying;

    private float _rollAngle;
    private float _passiveVerticalInput;
    private float _passiveYawInput;
    private float _passivePanX;
    
    private bool _freeFlightMode = false; public void SetFreeFlightMode(bool value) { _freeFlightMode = value; }

    private bool CameraInputEnabled => !GameManager.Instance.playerPaused;
    private bool Looking => CameraInputEnabled && (_isMouseLook || IsMouseDown);
    private bool IsMouseDown => Input.GetMouseButton(1);
    private bool IsClicking => Input.GetMouseButtonDown(1);
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        _wantedZoom = _minZoom;
        _cameraVector = new Vector3(0, 0, _wantedZoom);
        _initialSpaceHeight = _cameraSpace.transform.localPosition.y;
        _initialOffsetHeight = _cameraOffset.transform.localPosition.y;
    }


    private void PositionCamera()
    {
        // CURSED
        _cameraSpace.transform.localPosition = new Vector3(0, _initialSpaceHeight + _initialOffsetHeight * (1.0f - _spaceOffsetCorrection), 0);
        _cameraOffset.transform.localPosition = new Vector3(_passivePanX, _initialOffsetHeight * _spaceOffsetCorrection, 0);
        _cameraSpace.transform.localEulerAngles = new Vector3(_cameraVector.x, _cameraVector.y, 0);
        transform.localPosition = new Vector3(0, 0, -_cameraVector.z);
    }


    public void SetIsFlying(bool isFlying)
    {
        _isFlying = isFlying;
    }

    private void ApplyMouseLook()
    {
        if (!Looking)
        {
            return;
        }

        float mouseYaw = Input.GetAxis("Mouse X") * 2f;
        float mousePitch = Input.GetAxis("Mouse Y") * 2f;
        _cameraVector = new Vector3(
            Mathf.Clamp(_cameraVector.x - mousePitch, _minAngle, _maxAngle),
            Mathf.Clamp(_cameraVector.y + mouseYaw, -_maxYawOffset, _maxYawOffset),
            _cameraVector.z
        );
    }

    private void ApplyNeutralRecentering()
    {
        if (Looking)
        {
            return;
        }

        float pitchScale = _passiveVerticalInput >= 0f ? _passiveClimbPitchOffset : _passiveDescendPitchOffset;
        float targetPitch = -Mathf.Clamp(_passiveVerticalInput, -1f, 1f) * pitchScale;
        float targetYaw = Mathf.Clamp(_passiveYawInput, -1f, 1f) * _passiveYawLeadOffset;

        _cameraVector = new Vector3(
            Mathf.Lerp(_cameraVector.x, Mathf.Clamp(targetPitch, _minAngle, _maxAngle), Time.deltaTime * _neutralReturnSpeed),
            Mathf.Lerp(_cameraVector.y, Mathf.Clamp(targetYaw, -_maxYawOffset, _maxYawOffset), Time.deltaTime * _passiveYawLeadResponse),
            _cameraVector.z
        );
    }

    public void PlayerFlightInput(float verticalInput, float yawInput)
    {
        _passiveVerticalInput = Mathf.Clamp(verticalInput, -1f, 1f);
        _passiveYawInput = Mathf.Clamp(yawInput, -1f, 1f);
    }

    public void PlayerTurnInput(float leftRight)
    {
        leftRight = Mathf.Clamp(-leftRight, -15f, 15f);
        _rollAngle = Mathf.Lerp(_rollAngle, leftRight, Time.deltaTime * (Mathf.Abs(leftRight) < 5 ? 2f : 4f));
        _cameraOffset.transform.localEulerAngles = new Vector3(0, 0, _rollAngle);
    }

    private void ApplyPassivePan()
    {
        float targetPan = (Looking ? 0f : Mathf.Clamp(_passiveYawInput, -1f, 1f) * _passiveYawPanOffset);
        _passivePanX = Mathf.Lerp(_passivePanX, targetPan, Time.deltaTime * _passiveYawPanResponse);
    }

    private void TurnPlayer(float dx, float dy)
    {
        PlayerController.Instance.TryTurn(dx, dy);
    }
    // Update is called once per frame
    void Update()
    {
        
        if(IsClicking)
        {
            if(Time.time - _doubleClickTimer < _doubleClickTime || _isMouseLook)
            {
                _isMouseLook = !_isMouseLook;
                _freeFlightMode = !_freeFlightMode;
                _doubleClickTimer = 0;
            }
            else
            {
                _doubleClickTimer = Time.time;
            }
        }
        
        // check which mode we are in
        if(_freeFlightMode)
        {
            UpdateFreeFlightMode();
        }
        else
        {
            UpdateFlightMode();
        }

    }

    private void UpdateFlightMode()
    {
                
        
        if (!CameraInputEnabled)
        {
            _isMouseLook = false;
            _passiveVerticalInput = 0f;
            _passiveYawInput = 0f;
            _passivePanX = 0f;
            Cursor.lockState = CursorLockMode.None;
            PositionCamera();
            _spaceOffsetCorrection = Mathf.Lerp(_spaceOffsetCorrection, _isFlying ? 1 : 0, Time.deltaTime * 6f);
            return;
        }



        // zoom
        float wheelDelta = Input.mouseScrollDelta.y;
        _wantedZoom = Mathf.Clamp(_wantedZoom - wheelDelta * _zoomSpeed, _minZoom, _maxZoom);
        _cameraVector = new Vector3(_cameraVector.x, _cameraVector.y, Mathf.Lerp(_cameraVector.z, _wantedZoom, Time.deltaTime * 10f));

        ApplyMouseLook();
        ApplyNeutralRecentering();
        ApplyPassivePan();

        
        // _cameraVector += new Vector3(takeX, takeY);
        // _cameraVector = new Vector3(Mathf.Clamp(_cameraVector.x, _minAngle, _maxAngle), _cameraVector.y, _cameraVector.z);

        PositionCamera();

        Cursor.lockState = Looking ? CursorLockMode.Locked : CursorLockMode.None;

        _spaceOffsetCorrection = Mathf.Lerp(_spaceOffsetCorrection, _isFlying ? 1 : 0, Time.deltaTime * 6f);
    }

    private void UpdateFreeFlightMode()
    {
        // zoom
        float wheelDelta = Input.mouseScrollDelta.y;
        _wantedZoom = Mathf.Clamp(_wantedZoom - wheelDelta * _zoomSpeed, _minZoom, _maxZoom);
        _cameraVector = new Vector3(_cameraVector.x, _cameraVector.y, Mathf.Lerp(_cameraVector.z, _wantedZoom, Time.deltaTime * 10f));

        if(Looking)
        {
            float mouseX = Input.GetAxis("Mouse X") * 2f;
            float mouseY = Input.GetAxis("Mouse Y") * 2f;
            TurnPlayer(-mouseY, mouseX);
        }

        
        // _cameraVector += new Vector3(takeX, takeY);
        // _cameraVector = new Vector3(Mathf.Clamp(_cameraVector.x, _minAngle, _maxAngle), _cameraVector.y, _cameraVector.z);

        PositionCamera();

        Cursor.lockState = Looking ? CursorLockMode.Locked : CursorLockMode.None;

        _spaceOffsetCorrection = Mathf.Lerp(_spaceOffsetCorrection, _isFlying ? 1 : 0, Time.deltaTime * 6f);
    }
}
