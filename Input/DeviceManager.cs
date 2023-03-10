using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceManager : GenericSingleton<DeviceManager>
{
    [SerializeField] private GameObject _playerInputPrefab;
    [SerializeField] private InputActionAsset _defaultActionMap;

    [SerializeField] private List<PlayerInput> _players;
    [SerializeField] InputAction _joinAction;
    [SerializeField] InputAction _leaveAction;

    public event Action OnNewInput;

    private void Awake() {
        DontDestroyOnLoad(transform.root);
        PlayerInputManager.instance.playerPrefab = _playerInputPrefab;

        EnableJoining();
        EnableLeaving();

        _joinAction.performed += context => JoinAction(context); 
        _leaveAction.performed += context => LeaveAction(context);

        Debug.LogWarning("Dont forget to disable leaving and joining in playscene!");
    }
    private void OnDisable() {
        _joinAction.performed -= context => JoinAction(context);
        _leaveAction.performed -= context => LeaveAction(context);
    }


    private void JoinAction(InputAction.CallbackContext context) {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
        Debug.Log("Join called");
    }
    private void LeaveAction(InputAction.CallbackContext context) {
        for (int i = 0; i < _players.Count; i++)
        {
            foreach (var device in _players[i].devices)
            {
                if (context.control.device == device && device != null)
                {
                    DestroyPlayerInput(_players[i]);
                }
            }
        }
    }

    public void OnPlayerJoined(PlayerInput player) {
        _players.Add(player);
        OnNewInput?.Invoke();
    }
    public void OnPlayerLeft(PlayerInput player) {
        _players.Remove(player);
        OnNewInput?.Invoke();
    }

    public void EnableJoining() => _joinAction.Enable();
    public void EnableLeaving() => _leaveAction.Enable();

    public void DisableJoining() => _joinAction.Disable();
    public void DisableLeaving() => _leaveAction.Disable();

    public PlayerInput GetPlayerInput(int playerIndex) {

        if (playerIndex < 0 || playerIndex >= _players.Count)
            return null;
        else return _players[playerIndex];
    }
    public InputHandler GetInputHandler(int playerIndex) {
        if (playerIndex < 0 || playerIndex >= _players.Count)
            return null;
        else return _players[playerIndex].GetComponent<InputHandler>();
    }
    private void DestroyPlayerInput(PlayerInput player) {
        Destroy(player.gameObject);
    }
    public void DestroyAllPlayerInput() {
        for (int i = 0; i < _players.Count; i++)
        {
            Destroy(_players[i].gameObject);
        }
        _players.Clear();
    }

    public int GetPlayerCount() => _players.Count;
}
