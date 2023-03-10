using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]public class DialogueTrigger : MonoBehaviour {
    [SerializeField] private Quest _questWithDialogue;
    [SerializeField] private PlayerType _playerTypeToTrigger;
    [SerializeField] private BoxCollider _collider;

    private void Awake() {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        _questWithDialogue.HintTime = 0.01f;
        var player = other.GetComponentInParent<Player>();
        if (player != null) {
            if (_playerTypeToTrigger == player._type || _playerTypeToTrigger == PlayerType.Both) {
                GameManager.Instance.StartQuestTimer(_questWithDialogue);
                this.enabled = false;
            }
        }
    }
}