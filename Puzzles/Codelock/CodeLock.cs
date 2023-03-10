using System;
using System.Collections;
using System.Collections.Generic;
using Highlighters;
using UnityEngine;

public class CodeLock : InteractableObject
{
    //show when debugging
    public int[] codeLock = new int[4];
    public int currentPos;

    [Space, Header("Padlock Config"), Space]
    [SerializeField, Tooltip("should be 4 elements, represents the correct number for each dial")] private int[] _correctCode;
    private const int LowerCodeBounds = 0;
    private const int UpperCodeBounds = 9;
    [Header("Each dial gameObject for the lock")]
    [SerializeField] private Dial _dial1;
    [SerializeField] private Dial _dial2;
    [SerializeField] private Dial _dial3;
    [SerializeField] private Dial _dial4;
    
    private Player _player;
    private InputHandler _inputHandler;
    private float _lerpTimer;
    private float _inputTimer;
    private float _cooldown = 0.3f;
    private bool _rotatingLock;
    private Animator _anim;

    private bool _interacting;
    
    
    private Dial[] _dials = new Dial[4];

    private void OnEnable() {
        _dials[0] = (_dial1);
        _dials[1] = (_dial2);
        _dials[2] = (_dial3);
        _dials[3] = (_dial4);

        _anim = GetComponent<Animator>();
    }
    protected override void InteractAction(Player player) {
        _player = player;
        Debug.Log("interacting with codelock");
        _inputHandler = _player.InputHandler;
        _interacting = true;
        currentPos = 0;
        _dials[currentPos].Outline();
    }

    protected override void StopInteractAction(Player player)
    {
        _interacting = false;
        foreach (var dial in _dials)
        {
            {
                dial.StopOutline();
            }
        }
    }

    private void Update() {
        _inputTimer += Time.deltaTime;
        if (!_interacting) return;
        if (_inputHandler == null) return;
        
        var moveDir = _inputHandler._moveDir;
        if (moveDir == Vector3.zero || _rotatingLock) return;
        
        UpdateNumber(moveDir.z);
        if (_inputTimer < _cooldown) return;
        UpdatePos(moveDir.x);
    }
    private void UpdatePos(float xAxis) {
        switch (xAxis) {
            case > 0.8f:
                //_dials[currentPos].StopHighlight();
                _dials[currentPos].StopOutline();
                currentPos = (currentPos + 1) % codeLock.Length; // last index + 1 // check this later
                //_dials[currentPos].Highlight();
                _dials[currentPos].Outline();
                _inputTimer = 0;
                break;
            case < -0.8f: {
                //_dials[currentPos].StopHighlight();
                _dials[currentPos].StopOutline();
                currentPos--;
                if (currentPos < 0) {
                    currentPos = 3;
                }
                //_dials[currentPos].Highlight();
                _dials[currentPos].Outline();
                _inputTimer = 0;
                break;
            }
        }
    }
    private void UpdateNumber(float yAxis) {
        switch (yAxis) {
            case > 0.8f:
                codeLock[currentPos] = (codeLock[currentPos] + 1) % (UpperCodeBounds + 1);
                AudioManager.Instance.Play("PadlockSpin", transform.position);
                StartCoroutine(RotateDial(_dials[currentPos].gameObject, 1, 36));
                break;
            case < -0.8f:
                codeLock[currentPos]--;
                if (codeLock[currentPos] < LowerCodeBounds)
                    codeLock[currentPos] = UpperCodeBounds;
                AudioManager.Instance.Play("PadlockSpin", transform.position);
                StartCoroutine(RotateDial(_dials[currentPos].gameObject, 1, -36));
                break;
        }
        CheckCode();
    }
    private void CheckCode( ) {
        List<bool> matches = new List<bool>();

        for (int i = 0; i < codeLock.Length; i++) {
            if (codeLock[i] == _correctCode[i]) {
                matches.Add(true);
            }else {
             matches.Add(false);   
            }
        }
        if (matches.TrueForAll(match => match)) {
            Unlock();
        }
    }
    private void Unlock() {
        StopInteractAction(_player);
        _player.CurrentGameState = GameState.TransitionToNormal;
        _player.InteractionComponent.CanToggle = true;
        if (_quest != null) {
            _quest.CompleteQuest();
        }
        foreach (var codeEvents in events) {
            codeEvents?.Invoke();
        }
        foreach (var dial in _dials) {
            HighlightManager.Instance.RemoveFromOutline(dial.gameObject);
        }
       
        _anim.SetTrigger("Unlocked");
        AudioManager.Instance.Play("PadlockUnlock", transform.position);
        gameObject.layer = 0;
        
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(DestroyLock());
    }
    private IEnumerator RotateDial(GameObject dial, float timer, float change) {
        _rotatingLock = true;
        //change should be + or - 36
        _lerpTimer = timer;

        var targetRot = dial.transform.rotation * Quaternion.Euler(change,0,0);

        while (_lerpTimer >= 0f) {
            dial.transform.rotation = Quaternion.Slerp(targetRot,dial.transform.rotation, _lerpTimer / timer);
            
            _lerpTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _rotatingLock = false;
    }

    private IEnumerator DestroyLock() {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

}
