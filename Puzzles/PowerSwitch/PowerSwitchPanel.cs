using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PowerSwitchPanel : InteractableObject
{
    [SerializeField] private PowerSwitchManager _manager;
    
    public Switch[] Switches = new Switch[4];
    public Switch CurrentSwitch;
    [SerializeField] private int SwitchIndex = 0;
    private float _timer;
    private float _cooldown = 0.5f;

    private Player _player;

    [SerializeField] private bool _interacting = false;
    
    protected override void InteractAction(Player player) {
        
        _player = player;
        SwitchIndex = 0;
        CurrentSwitch = Switches[SwitchIndex];
        _interacting = true;
        CurrentSwitch.Outline();
    }

    protected override void StopInteractAction(Player player)
    {
        _interacting = false;
        foreach (var switchObj in Switches)
        {
            switchObj.StopOutline();
        }
    }

    private void Update() {
        
        if (_player == null) 
            return;
        _timer += Time.deltaTime;
        if (_player.InputHandler._moveDir == Vector3.zero || _timer < _cooldown) 
            return;
        if (!_interacting)
            return;
  
        UpdatePos(_player.InputHandler._moveDir.x);
        UpdateValue(_player.InputHandler._moveDir.z);
    }

    private void UpdatePos(float xAxis) {
        switch (xAxis) {
            case > 0.5f:
                Switches[SwitchIndex].StopOutline();
                SwitchIndex = SwitchIndex + 1;
                if (SwitchIndex > 3) {
                    SwitchIndex = 0;
                }
                Switches[SwitchIndex].Outline();
                _timer = 0;
                break;
            case < -0.5f: {
                Switches[SwitchIndex].StopOutline();
                SwitchIndex = SwitchIndex - 1;
                if (SwitchIndex < 0) {
                    SwitchIndex = Switches.Length - 1;
                }
                Switches[SwitchIndex].Outline();
                _timer = 0;
                break;
            }
        }
        CurrentSwitch = Switches[SwitchIndex];
    }
    private void UpdateValue(float zAxis) {
        if (zAxis > 0.6f) {
            if (Switches[SwitchIndex].Activated) return;
            Switches[SwitchIndex].Activate();
            _timer = 0;
            
            if (!Switches[SwitchIndex].IncludedInPuzzle) return;
            _manager.InputOrder[_manager.CurrentInput] = Switches[SwitchIndex].CorrespondingNumber;
            _manager.CurrentInput++;
            Debug.Log(_manager.CurrentInput);
        }
        if (_manager.CheckIfCorrect()) {
            _manager.Unlock();
        }
    }
    public void ExitPuzzle() {
        StopInteractAction(_player);
        _player.CurrentGameState = GameState.TransitionToNormal;
    }
}
