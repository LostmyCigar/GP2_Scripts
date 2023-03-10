using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;

public class Tv : InteractableObject
{
    public bool CanBeTurnedOn = false; //Power puzzle should change this?
    private bool _interacting = false;
    private Player _player;
    private InputHandler _inputHandler;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _codeText;
    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private float _keyTextTimer;
    
    
    //Actual Game Thingies
    public bool InputEnabled = true;
    [SerializeField] private Camera _gameCamera;
    public GameObject PlayerChar;
    public List<Bottles> BottlesList = new List<Bottles>();
    public GameObject Key;    
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;
    public Animator PlayerAnimator;

    public bool FollowCam = false;
    [SerializeField] private List<Transform> _cameraPositions = new List<Transform>();
    [HideInInspector] public Animator ScreenAnimator;

    //TODO Make camera follow when in labyrinth
    //TODO setup different positions in _cameraPositions
    //TODO Animations for transitions between room

    private void OnEnable() {
        Key.SetActive(false);
        _codeText.enabled = false;
        _keyText.color = new Color(_keyText.color.r, _keyText.color.g, _keyText.color.b, 0f);
        ScreenAnimator = _image.GetComponent<Animator>();
    }

    protected override void InteractAction(Player player) {
        if(!CanBeTurnedOn) return;
        _player = player;
        _interacting = true;
        
        _inputHandler = _player.InputHandler;
        _rb = PlayerChar.GetComponent<Rigidbody2D>();
        PlayerAnimator = PlayerChar.GetComponent<Animator>();

        ScreenAnimator.SetBool("TurnedOn", _interacting);
    }

    protected override void StopInteractAction(Player player) {
        _interacting = false;
        ScreenAnimator.SetBool("TurnedOn", _interacting);
    }

    private void Update() {
        if (!_interacting) return;
        if (!InputEnabled) return;
        if (_inputHandler == null) return;

        var moveDir = _inputHandler._moveDir;

        HandleMovement( moveDir);
        HandleAnimation(moveDir);
    }

    private void HandleAnimation(Vector3 moveDir) {
        PlayerAnimator.SetFloat("movedir.x", moveDir.x);
        PlayerAnimator.SetFloat("movedir.z", moveDir.z);
    }

    private void HandleMovement(Vector3 movedir) {
        var movedirx = Mathf.Sign(movedir.x);
        var movediry = Mathf.Sign(movedir.z);

        if (movedir.x <= 0.3 && movedir.x >= -0.3) {
            movedirx = 0;
        }
        if (movedir.z <= 0.3 && movedir.z >= -0.3) {
            movediry = 0;
        }
        
        _rb.velocity = new Vector3(movedirx, movediry, 0).normalized * _speed;
    }
    
    private IEnumerator FadeText()
    {
        Color startColor = _keyText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        float t = 0f;

        while (t < _keyTextTimer)
        {
            t += Time.deltaTime;
            float normalizedTime = t / _keyTextTimer;
            _keyText.color = Color.Lerp(startColor, endColor, normalizedTime);
            yield return null;
        }

        _keyText.color = endColor;

        yield return new WaitForSeconds(3f);

        startColor = endColor;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        t = 0f;

        while (t < _keyTextTimer)
        {
            t += Time.deltaTime;
            float normalizedTime = t / _keyTextTimer;
            _keyText.color = Color.Lerp(startColor, endColor, normalizedTime);
            yield return null;
        }
        _keyText.color = endColor;
    }

    public void CheckWin() {
        if (CheckComplete()) {
            Debug.Log("win");
            Key.SetActive(true);
            _quest.CompleteQuest();
            StartCoroutine(FadeText());
        } 
    }

    public void CheckNewRoom(int roomNumber) {
        var gameCam = _gameCamera.transform;

        if (FollowCam) {
            gameCam.parent = PlayerChar.transform;
            gameCam.localPosition = new Vector3(0, 0, -3);
        }
        else {
            gameCam.parent = null;
            gameCam.position = _cameraPositions[roomNumber].position;
        }
    }

    public void ExitPuzzle() {
        StopInteractAction(_player);
        _player.CurrentGameState = GameState.TransitionToNormal;
        ScreenAnimator.SetBool("TurnedOn", false);
        StartCoroutine(ShowCode());
        CanBeTurnedOn = false;
        foreach (var codeEvents in events) {
            codeEvents?.Invoke();
        }
    }
    
    private IEnumerator ShowCode() {
        yield return new WaitForSeconds(2);
        _codeText.enabled = true;
    }

    public void PowerSwitchOn() {
        //Called from event list of power quest
        CanBeTurnedOn = true;
    }

    public bool CheckComplete() => BottlesList.TrueForAll(bottle => bottle.Collected);
}

public interface IcollectibleBottle
{
    public void AddSelf(Bottles bottle) {
    }
}