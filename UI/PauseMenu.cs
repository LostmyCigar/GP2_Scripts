using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour {
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _endScreen;
    
    private bool _isPaused = false;
    private bool _gameEnded = false;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private List<InputHandler> _inputHandlers;

    private void Awake() {
        _inputHandlers.Add(DeviceManager.Instance.GetInputHandler(0));
        _inputHandlers.Add(DeviceManager.Instance.GetInputHandler(1));
        _pauseUI.SetActive(false);
    }

    private void Update() {

        foreach (var item in _inputHandlers) {
            if (_gameEnded) return;
            if (item._pause)
            {
                Pause();
                item.UsePauseInput();
            }
        }
    }

    private void Pause()
    {
        _isPaused = !_isPaused;
        _pauseUI.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;
    }

    public void EndScreenCoroutine() {
        _gameEnded = true;
        StartCoroutine(EnableEndScreen());
    }

    private IEnumerator EnableEndScreen() {
        yield return new WaitForSeconds(3);
        Time.timeScale = 0;
        _endScreen.SetActive(true);
    }
    
    public void MouseToggle(InputAction.CallbackContext context) {
        if (context.started) {
            if (_playerInput.currentControlScheme == "KeyboardMouse") {
                Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = _isPaused;
            }
        }
    }
    public void OnResume() {
        _isPaused = !_isPaused;
        _pauseUI.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;
    }
    public void OnQuit() {
        Application.Quit();
    }
}
