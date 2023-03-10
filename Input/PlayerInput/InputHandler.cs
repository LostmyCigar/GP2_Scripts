using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector3 _moveDir;
    public Vector2 _mouseDelta;
    public bool _interact;
    public bool _dropOff;
    public bool _pause;
    public bool _toggleLeft;
    public bool _toggleRight;

    private void Awake()
    {
        DontDestroyOnLoad(transform.root);
    }

    public void MouseDeltaInput(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        _moveDir.x = context.ReadValue<Vector2>().x;
        _moveDir.z = context.ReadValue<Vector2>().y;
        _moveDir.Normalize();
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
            _interact = true;
        else if (context.canceled)
            _interact = false;
    }

    public void PauseInput(InputAction.CallbackContext context)
    {
        _pause = context.performed;
    }

    public void DropOffInput(InputAction.CallbackContext context)
    {
        _dropOff = context.performed;
    }

    public void ToggleLeft(InputAction.CallbackContext context)
    {
        if (context.started)
            _toggleLeft = true;
        else if (context.canceled)
            _toggleLeft = false;
    }
    public void ToggleRight(InputAction.CallbackContext context)
    {
        if (context.started)
            _toggleRight = true;
        else if (context.canceled)
            _toggleRight = false;
    }

    public void UseInteractInput() => _interact = false;
    public void UseDropOffInput() => _dropOff = false;
    public void UsePauseInput() => _pause = false;
    public void UseToggleInputs()
    {
        _toggleRight = false;
        _toggleLeft = false;
    }
}
