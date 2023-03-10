using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private GameState _currentGameState = GameState.NormalState;
    
    public GameState CurrentGameState {
        get => _currentGameState;
        set {
            if (value != _currentGameState) {
                _currentGameState = value;
                _cameraScript.Initialize(value);
            } else {
                _currentGameState = value;
            }
        }
    }

   
    [SerializeField] private int _playerIndex;
    [SerializeField] private GhostData _ghostData;
    [SerializeField] private ThridPersonCameraData _cameraData;
    [SerializeField] public PlayerType _type;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private LayerMask _blockingLayer;
    [SerializeField] private Animator _animator;

    private Camera _camera;
    private ThirdPersonCamera _cameraScript;
    private Rigidbody _rigidbody;
    public InputHandler InputHandler { get; private set; }

    private List<PlayerComponent> _playerComponents = new List<PlayerComponent>();
    public ThirdPersonMovement _movementComponent;
    public PlayerInteraction InteractionComponent;
    public PlayerInventory InventoryComponent;

    private int cachedComponentCount;

    private void Awake() {
        if (_type == PlayerType.Ghost)
            _camera = GameObject.Find("DogCamera").GetComponentInChildren<Camera>(); // we should do this in inspector since its only 1 scene
        else
            _camera = GameObject.Find("HumanCamera").GetComponentInChildren<Camera>();
            
        _rigidbody = GetComponent<Rigidbody>();
        _cameraScript = _camera.GetComponent<ThirdPersonCamera>();
        InputHandler = DeviceManager.Instance.GetInputHandler(_playerIndex);

        if (InputHandler == null)
            return;

        Physics.IgnoreCollision(GameObject.Find("PlayerGhost").GetComponentInChildren<Collider>(), gameObject.GetComponentInChildren<Collider>()); //cursed

        _animator = GetComponent<Animator>();
        _movementComponent = new ThirdPersonMovement(this, _camera, InputHandler, _ghostData, _rigidbody, _type, _animator);
        InteractionComponent = new PlayerInteraction(this, InputHandler, _type, _interactableLayer, _blockingLayer, _ghostData.InteractionRange);
        InventoryComponent = new PlayerInventory(transform, this, InputHandler);

        AddComponent(_movementComponent);
        AddComponent(InteractionComponent);
        AddComponent(InventoryComponent);
    }

    #region Components (Adds and runs player components) 
    private void Start()
    {
        StartComponents();
    }

    private void Update()
    {
        LogicUpdateComponents();
    }
    private void FixedUpdate()
    {
        PhysicsUpdateComponents();
    }

    private void StartComponents() {
        for (int i = 0; i < cachedComponentCount; i++)
        {
            _playerComponents[i].StartComponent();
        }
    }
    private void LogicUpdateComponents() {
        for (int i = 0; i < cachedComponentCount; i++)
        {
            _playerComponents[i].LogicUpdate();
        }
    }
    private void PhysicsUpdateComponents() {
        for (int i = 0; i < cachedComponentCount; i++)
        {
            _playerComponents[i].PhysicsUpdate();
        }
    }

    private void AddComponent(PlayerComponent component) {
        _playerComponents.Add(component);
        cachedComponentCount++;
    }

    #endregion 

    public void UpdateCameraTransform(Transform cameraTransform = null) {
        _cameraScript.UpdateCameraTransform(cameraTransform);
    }


    #region Gizmos and gadgets (OnDrawGizmos)
    private void OnDrawGizmos()
    {

        DrawDropping();

        if (InteractionComponent == null)
            return;
        DrawInteraction();
    }

    private void DrawDropping()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.2f);
        }

        Gizmos.DrawRay(ray);
    }
    private void DrawInteraction()
    {
        if (InventoryComponent.HeldItem == null)
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(transform.position + Vector3.up, 0.2f);


        Gizmos.color = Color.green;
        if (InteractionComponent.CurrentInteractable != null)
            Gizmos.DrawSphere(InteractionComponent.CurrentInteractable.transform.position, 0.2f);

        Gizmos.color = Color.grey;

        Gizmos.DrawWireSphere(transform.position, _ghostData.InteractionRange);
        DrawCloseInteractables();

    }

    private void DrawCloseInteractables()
    {

        var colliders = Physics.OverlapSphere(transform.position, _ghostData.InteractionRange, _interactableLayer);
        List<InteractableObject> interactables = new List<InteractableObject>();
        foreach (Collider col in colliders)
        {
            var interactable = col.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (interactable.HideInteraction)
                    continue;

                if (Vector3.Distance(col.transform.position, transform.position) < _ghostData.InteractionRange)
                {
                    if (interactables.Contains(interactable))
                        continue;
                    if (!interactable.ComparePlayerType(_type))
                        continue;
                    interactables.Add(interactable);
                }
            }
        }

        foreach (var item in interactables)
        {
            Gizmos.DrawWireCube(item.transform.position, new Vector3(1, 1, 1));

            if (!item.IsVisible() || !item.InLineOfSight(this, _blockingLayer))
                Gizmos.color = Color.red;
            else Gizmos.color = Color.green;

            Gizmos.DrawLine(item.transform.position, transform.position);
        }
    }
    #endregion
}
