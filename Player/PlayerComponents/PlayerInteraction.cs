using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : PlayerComponent
{
    private PlayerType _interActionType;
    private LayerMask _interactionLayers; // could use scriptableobjects for some of these
    private LayerMask _blockingLayer;
    private InputHandler _inputHandler;
    private Player _player;
    private float _interactionRange;

    private bool hasInteractableSelected;
    private int currentInteractableIndex;

    public bool CanToggle = true;
    public bool InteractionLocked;
    public InteractableObject CurrentInteractable;
    public List<InteractableObject> CloseInteractables = new List<InteractableObject>();


    public PlayerInteraction(Player player, InputHandler inputHandler, PlayerType interactionType,
        LayerMask interactionLayer, LayerMask blockingLayer, float interactionRange)
    {
        _inputHandler = inputHandler;
        _player = player;
        _interActionType = interactionType;
        _interactionLayers = interactionLayer;
        _interactionRange = interactionRange;
        _blockingLayer = blockingLayer;
    }

    public override void LogicUpdate()
    {
        if (InteractionLocked)
            return;

        GetCloseInteractables();
        SelectClosestInteractable();
        RemoveFarInteractables();

        if (_inputHandler._toggleLeft)
            Toggle(-1);

        if (_inputHandler._toggleRight)
            Toggle(1);
        
        if (_inputHandler._interact)
            Interact();

        if (_inputHandler._dropOff)
            DropOff();

    }

    private void GetCloseInteractables()
    {
        var colliders = Physics.OverlapSphere(_player.transform.position, _interactionRange, _interactionLayers);

        foreach (Collider col in colliders)
        {

            var interactable = col.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (interactable.HideInteraction) //we should redesign this to not need 1000 guardclauses later
                    continue;
                if (!interactable.IsVisible())
                    continue;
                if (!interactable.InLineOfSight(_player, _blockingLayer))
                    continue;
                if (!interactable.ComparePlayerType(_interActionType))
                    continue;

                if (Vector3.Distance(col.transform.position, _player.transform.position) < _interactionRange)
                    AddCloseInteractable(interactable);
            }
        }
    }
    private void RemoveFarInteractables()
    {
        for (int i = 0; i < CloseInteractables.Count; i++)
        {
            var distance = Vector3.Distance(CloseInteractables[i].transform.position, _player.transform.position);
            if (distance > _interactionRange || !CloseInteractables[i].IsVisible() || !CloseInteractables[i].InLineOfSight(_player, _blockingLayer))
            {
                if (CloseInteractables[i] == CurrentInteractable) //check if we are removing current interactable (only reason to set hasInteractableSelected to false)
                    hasInteractableSelected = false;
                RemoveInteractable(i);
            }
        }
    }

    private void Toggle(int indexChange)
    {
        if (!CanToggle)
            return;
        if (CloseInteractables.Count == 0)
            return;

        hasInteractableSelected = true;
        _inputHandler.UseToggleInputs();

        currentInteractableIndex = CloseInteractables.IndexOf(CurrentInteractable);
        currentInteractableIndex += indexChange;

        if (currentInteractableIndex < 0)
            currentInteractableIndex = CloseInteractables.Count - 1;

        if (CloseInteractables.Count != 0)
            currentInteractableIndex = currentInteractableIndex % CloseInteractables.Count;

        ChangeCurrentInteractable(CloseInteractables[currentInteractableIndex]);
    }
    private void SelectClosestInteractable()
    {
        if (hasInteractableSelected)
            return;

        InteractableObject closest = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < CloseInteractables.Count; i++)
        {
            float distance = Vector3.Distance(_player.transform.position, CloseInteractables[i].transform.position);

            if (distance < closestDistance)
            {
                closest = CloseInteractables[i];
                closestDistance = distance;
            }
        }

        if (closest != null)
        {
            if (closest == CurrentInteractable)
                return;

            if (CurrentInteractable != null)
            {
                CurrentInteractable.StopHighlight();
            }

            CurrentInteractable = closest;
            CurrentInteractable.Highlight();
        } else CurrentInteractable = null;
    }

    private void ChangeCurrentInteractable(InteractableObject newInteractable)
    {
        CurrentInteractable.StopHighlight();
        CurrentInteractable.Outline();
        CurrentInteractable = newInteractable;
        CurrentInteractable.Highlight();
    }
    private void RemoveInteractable(int index)
    {
        CloseInteractables[index].StopHighlight();
        CloseInteractables[index].StopOutline();
        CloseInteractables[index].DisableHighlighter();
        CloseInteractables.RemoveAt(index);
    }
    private void AddCloseInteractable(InteractableObject interactable)
    {
        if (CloseInteractables.Contains(interactable))
            return;

        CloseInteractables.Add(interactable);
        interactable.Outline();
    }
    public void Interact()
    {
        _inputHandler.UseInteractInput();
        if (CurrentInteractable != null) {
            CurrentInteractable.Interact(_player, _interActionType);
        }        
    }

    public void DropOff()
    {
        _inputHandler.UseDropOffInput();
        if (CurrentInteractable != null) {
            CurrentInteractable.StopInteract(_player, _interActionType);
        }
    }
}
