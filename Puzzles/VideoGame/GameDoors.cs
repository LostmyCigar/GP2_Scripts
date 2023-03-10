using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDoors : MonoBehaviour
{
    public int LeadsToRoomNumber;
    [SerializeField] private Tv _tvReference;
    [SerializeField] private bool _toLabyrinth;
    
    public Transform LeadsTo;

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Player")) {
            StartCoroutine(MoveCharacter());
        }
    }

    private IEnumerator MoveCharacter() {
        _tvReference.InputEnabled = false;
        _tvReference.ScreenAnimator.SetTrigger("RoomTransition");
        yield return new WaitForSeconds(0.4f);
        _tvReference.PlayerChar.transform.position = LeadsTo.position;
        _tvReference.FollowCam = _toLabyrinth;
        _tvReference.CheckNewRoom(LeadsToRoomNumber);
        _tvReference.InputEnabled = true;
    }
}
