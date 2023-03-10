using Highlighters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour {
    //This should be way shorter and only contain interactablility, but SOLID is overated and also its to late to change now :(
    [SerializeField] protected Quest _quest;
    [SerializeField] protected PlayerType _canInteractWith;
    [SerializeField] protected CameraMode _cameraMode;
    [SerializeField] protected InteractableObjectsHighlight _highlightSettings;
    [SerializeField] protected bool _isHighlightable;
    public bool HideInteraction;
    private Player _interactionPlayer;
    private LayerMask _mask;
    private LayerMask GhostHighlight => LayerMask.GetMask();
    private LayerMask HumanHighlight => LayerMask.GetMask();
    private LayerMask BothHighlight => LayerMask.GetMask();

    public int GetMask() {
        switch (_canInteractWith) {
            case PlayerType.Both:
                return 13;
            case PlayerType.Ghost:
                return 14;
            case PlayerType.Human:
                return 15;
               
        }

        return LayerMask.GetMask("Default");
    }

    [SerializeField, Tooltip("If cameraType is Puzzlemode")]
    protected Transform _cameraTransform;

    [SerializeField] protected List<UnityEvent> events;

    protected MeshRenderer _meshRenderer;
    protected Highlighter _highlighter;
    protected Collider _collider;
    protected bool highlighted;
    public bool TriggerEventOnInteract;
    [SerializeField] protected GameObject[] _renderObject;

    private void Awake() {
        if (_quest != null) {
            _quest.Initialize(this);
        }
        
        // _renderObject = GetComponent<Renderer>();
        // if (_renderObject == null) {
        //     _renderObject = GetComponentInChildren<Renderer>();
        // }

        if (_renderObject != null && _renderObject.Length > 0) {
            _mask = _renderObject[0].gameObject.layer;
        }

        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }


    public void MakeInteractable() => HideInteraction = false;

    public void Interact(Player player, PlayerType type) {
        if (type != _canInteractWith && _canInteractWith != PlayerType.Both)
            return;

        if (_interactionPlayer != null)
            return;

        if (player.CurrentGameState == GameState.TransitionToPuzzle ||
            player.CurrentGameState == GameState.TransitionToNormal)
            return;

        if (TriggerEventOnInteract) {
            foreach (var @event in events) {
                @event?.Invoke();
            }
        }

        if (_cameraMode == CameraMode.Puzzle) {
            player.UpdateCameraTransform(_cameraTransform);
            EnterPuzzleCameraMode(player);
        }

        InteractAction(player);
    }

    public void StopInteract(Player player, PlayerType type) {
        if (_interactionPlayer != player)
            return;

        if (player.CurrentGameState == GameState.TransitionToPuzzle ||
            player.CurrentGameState == GameState.TransitionToNormal)
            return;

        if (_cameraMode == CameraMode.Puzzle) {
            player.UpdateCameraTransform(_cameraTransform);
            ExitPuzzleCameraMode(player);
            StopInteractAction(player);
        }
    }

    protected virtual void InteractAction(Player player) {
    }

    protected virtual void StopInteractAction(Player player) {
    }

    public void CompleteQuest() {
        _quest.CompleteQuest();
    }

    public bool IsVisible() {
        return _meshRenderer.isVisible;
    }

    public bool ComparePlayerType(PlayerType type) {
        if (_canInteractWith == PlayerType.Both)
            return true;

        return type == _canInteractWith;
    }

    public bool InLineOfSight(Player player, LayerMask blockingLayers) {
        return !Physics.Linecast(player.transform.position, transform.position, blockingLayers);
    }


    #region Camera modes

    //Choosing camera could be redesigned a bit :)
    protected void EnterPuzzleCameraMode(Player player) {
        _isHighlightable = false;
        StopHighlight();
        StopOutline();
        player.InteractionComponent.CanToggle = false;
        _interactionPlayer = player;

        ToInteraction(player);
    }

    protected void ExitPuzzleCameraMode(Player player) {
        FromInteraction(player);

        player.InteractionComponent.CanToggle = true;
        _isHighlightable = true;
        _interactionPlayer = null;
    }

    private void ToInteraction(Player player) {
        if (_cameraMode == CameraMode.Puzzle && player.CurrentGameState == GameState.NormalState) {
            player.CurrentGameState = GameState.TransitionToPuzzle;
        }

        if (_quest != null && _quest.CurrentQuestState == QuestState.Completed) {
            if (events == null || events.Count < 1) return; // ska ske engång när du klarat av questet. 
            foreach (var gameEvent in events) {
                gameEvent?.Invoke();
            }
        }
    }

    private void FromInteraction(Player player) {
        if (player.CurrentGameState == GameState.PuzzleState) {
            player.CurrentGameState = GameState.TransitionToNormal;
        }
    }

    #endregion


    #region Highlight

    public void Outline() {
        if (!_isHighlightable)
            return;

        // HighlightManager.Instance.AddToOutline(gameObject);
    }

    public void StopOutline() {
        //  HighlightManager.Instance.RemoveFromOutline(gameObject);
    }

    public void Highlight() {
        if (_renderObject == null || _renderObject.Length == 0) {
            return;
        }

        if (!_isHighlightable)
            return;

        //HighlightManager.Instance.AddToHighlight(gameObject);
        foreach (var renderObject in _renderObject) {
            renderObject.layer = GetMask();
        }
    }

    public void StopHighlight() {
        if (_renderObject == null || _renderObject.Length == 0) {
            return;
        }

        // HighlightManager.Instance.RemoveFromHighlight(gameObject);
        foreach (var renderObject in _renderObject) {
            renderObject.layer = _mask;
        }
    }

    public void DisableHighlighter() {
        HighlightManager.Instance.RemoveFromHighlight(gameObject);
        HighlightManager.Instance.RemoveFromOutline(gameObject);
    }

    #endregion
}

public enum PlayerType {
    Ghost,
    Human,
    Both
}

public enum CameraMode {
    Normal,
    Puzzle
}