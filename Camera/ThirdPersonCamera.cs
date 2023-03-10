using System.Collections;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField] private Player _player;
    [SerializeField] private int _playerIndex;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private ThridPersonCameraData _cameraData;
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private Transform _pivotPoint;
    private Camera _camera;
    

    //cached
    private Vector3 _defaultPosition = Vector3.zero; //clarity
    private float _lookAngle;
    private float _pivotAngle;
    private Vector3 _rotation;
    private Vector3 _targetPosition;
    
    private Vector3 _startPosition;
    private Quaternion _startRot;

    [SerializeField] private Transform _puzzleCameraTransform; // debugg

    private float _lerpTimer;
    private bool _lockCamera;

    private int _originalCullingMask;

    private void Awake()
    {
        _inputHandler = DeviceManager.Instance.GetInputHandler(_playerIndex);
    }

    private void Start()
    {
        if (_inputHandler == null)
            Debug.LogWarning("Assign an input source to Camera. (Add the correct player input to the inspector field ;) )");

        _camera = gameObject.GetComponent<Camera>();
        _originalCullingMask = _camera.cullingMask;
    }

    private void LateUpdate() {
        if (_lockCamera) return;
        RotateCamera();
        HandleCollisions();
    }

    private void RotateCamera() {

        var mouseX = _inputHandler._mouseDelta.x;
        var mouseY = _inputHandler._mouseDelta.y;

        _lookAngle += (mouseX * _cameraData._cameraLookSpeed) * Time.deltaTime;
        _pivotAngle += (mouseY * _cameraData._cameraLookSpeed) * Time.deltaTime;
        _pivotAngle = Mathf.Clamp(_pivotAngle, _cameraData._cameraPivotMin, _cameraData._cameraPivotMax);

        _rotation.y = _lookAngle * (_cameraData._invertedLook ? 1 : -1); //Bit cursed but w/e
        _rotation.x = -_pivotAngle * (_cameraData._invertadPivot ? 1 : -1);
        Quaternion rotationQuaternion = Quaternion.Euler(_rotation);
        _baseTransform.rotation = rotationQuaternion;
    }

    private void HandleCollisions() { //performance is overrated
        _targetPosition = _pivotPoint.position;
        RaycastHit hit;
        Vector3 targetDir = (_pivotPoint.position - _baseTransform.position).normalized;
        float distance = Vector3.Distance(_baseTransform.position, _pivotPoint.position);

        if (Physics.SphereCast(_baseTransform.position, _cameraData._collisionRange,
            targetDir, out hit, distance + _cameraData._collisionOffset, _cameraData._collisionLayers))
        {
            _targetPosition = hit.point + hit.normal * _cameraData._collisionOffset;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.1f);
    }
    private void GetInput(int playerIndex)
    {
        _inputHandler = DeviceManager.Instance.GetInputHandler(playerIndex);
    }

    [ContextMenu("GetInput")]
    public void GetInputMenuItem()
    {
        GetInput(_playerIndex);
    }
    private void OnDrawGizmos()
    {
        RaycastHit hit;
        Vector3 targetDir = (_pivotPoint.position - _baseTransform.position).normalized;
        float distance = Vector3.Distance(_baseTransform.position, _pivotPoint.position);

        if (Physics.SphereCast(_baseTransform.position, _cameraData._collisionRange,
            targetDir, out hit, distance + _cameraData._collisionOffset, _cameraData._collisionLayers))
        { 
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(_baseTransform.position, _pivotPoint.position);
        
    }
    public void Initialize(GameState currentState) {
        switch (currentState) {
            case GameState.NormalState:
                _camera.cullingMask = _originalCullingMask;
                _lockCamera = false;
                break;
            case GameState.PuzzleState:
                _camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
                _lockCamera = true;
                break;
            case GameState.TransitionToNormal:
                if (_puzzleCameraTransform ==  null) return;
                _camera.cullingMask = _originalCullingMask;
                _lockCamera = true;
                StartCoroutine(LerpCamera(_puzzleCameraTransform.position, _puzzleCameraTransform.rotation, _startPosition, _startRot,  1, currentState));
                break;
            case GameState.TransitionToPuzzle:
                if (_puzzleCameraTransform ==  null) return;

                _lockCamera = true;
                SaveCameraPosition();
                StartCoroutine(LerpCamera(_startPosition, _startRot,_puzzleCameraTransform.position, _puzzleCameraTransform.rotation,  1, currentState));
                break;
        }
    }
    private void SaveCameraPosition()
    {
        _startPosition = transform.position;
        _startRot = transform.rotation;
    }
    private IEnumerator LerpCamera(Vector3 startPos, Quaternion startRot, Vector3 targetPos, Quaternion targetRot, float lerpTime, GameState state) {
        _lerpTimer = lerpTime;
        while (_lerpTimer >= 0f) {
            transform.position = Vector3.Lerp(targetPos, startPos, _lerpTimer / lerpTime);
            transform.rotation = Quaternion.Lerp(targetRot, startRot, _lerpTimer / lerpTime);
            
            _lerpTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (state == GameState.TransitionToNormal) {
            _player.CurrentGameState = GameState.NormalState;
        }
        else if(state == GameState.TransitionToPuzzle) {
            _player.CurrentGameState = GameState.PuzzleState;
        }
    }
    
    public void UpdateCameraTransform(Transform cameraTransform = null) {
        _puzzleCameraTransform = cameraTransform;
    }
}
