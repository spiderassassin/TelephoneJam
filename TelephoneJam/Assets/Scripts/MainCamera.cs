using TMPro;
using Unity.VisualScripting;
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
    private float _zoomSpeed = 1f;
    [SerializeField]
    private float _doubleClickTime = 0.25f;

    private float _doubleClickTimer = 0;
    private bool _isMouseLook = false;
    private float _wantedZoom;
    private Vector3 _cameraVector; // x / y = xy rotation in degrees

    private float _initialSpaceHeight;
    private float _initialOffsetHeight;
    private float _spaceOffsetCorrection;
    private bool _isFlying;

    private float _rollAngle;

    private bool Looking => _isMouseLook || IsMouseDown;
    private bool IsMouseDown => Input.GetMouseButton(0) || Input.GetMouseButton(1);
    private bool IsClicking => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
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
        _cameraOffset.transform.localPosition = new Vector3(0, _initialOffsetHeight * _spaceOffsetCorrection, 0);
        _cameraSpace.transform.localEulerAngles = new Vector3(_cameraVector.x, _cameraVector.y, 0);
        transform.localPosition = new Vector3(0, 0, -_cameraVector.z);
    }


    public void SetIsFlying(bool isFlying)
    {
        _isFlying = isFlying;
    }

    private void TurnPlayer(float dx, float dy)
    {
        PlayerController.Instance.TryTurn(dx, dy);
    }

    public void PlayerTurnInput(float leftRight)
    {
        leftRight = Mathf.Clamp(-leftRight, -15f, 15f);
        _rollAngle = Mathf.Lerp(_rollAngle, leftRight, Time.deltaTime * (Mathf.Abs(leftRight) < 5 ? 2f : 4f));
        _cameraOffset.transform.localEulerAngles = new Vector3(0, 0, _rollAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsClicking)
        {
            if(Time.time - _doubleClickTimer < _doubleClickTime || _isMouseLook)
            {
                _isMouseLook = !_isMouseLook;
                _doubleClickTimer = 0;
            }
            else
            {
                _doubleClickTimer = Time.time;
            }
        }

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
