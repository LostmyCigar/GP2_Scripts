using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]public class QuestCompleteTrigger : MonoBehaviour {
    [SerializeField] private Quest _questToComplete;
    [SerializeField] private PlayerType _playerType;
    [SerializeField] private BoxCollider _collider;

    private void Awake() {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponentInParent<Player>();
        if (player != null) {
            if (_playerType == player._type || _playerType == PlayerType.Both) {
                _questToComplete.CompleteQuest();
                this.enabled = false;
            }
        }
    }
}