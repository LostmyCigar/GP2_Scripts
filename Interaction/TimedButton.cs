using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedButton : InteractableObject {
    public AudioClip Sound;
    public float TimeActive = 5;
    public bool Activated;
    private float _timer;
    private Player _player;
    private InputHandler _inputHandler;

    protected override void InteractAction(Player player) {
        _player = player;
        _inputHandler = _player.InputHandler;
        if (_quest != null) {
            _quest.CompleteQuest();
        }
    
        events.ForEach(buttonEvent => buttonEvent?.Invoke());

        Activated = true;
        Debug.Log("Got through interaction on doorbell");
    }

    private void Update() {
        if (!Activated) return;
        _timer += Time.deltaTime;
        if (_timer > TimeActive) {
            if (_quest == null) {
                return;
            }
            if (_quest.LinkedQuest.CurrentQuestState != QuestState.Completed) {
                _quest.CurrentQuestState = QuestState.Active;
                _quest.LinkedQuest.CurrentQuestState = QuestState.Active;
            }

            Activated = false;
        }
    }
}