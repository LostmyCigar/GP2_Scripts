using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bottles : MonoBehaviour, IcollectibleBottle
{
    private Tv _tvScript;
    public bool Collected;
    private void Awake() {
        _tvScript = FindObjectOfType<Tv>().GetComponent<Tv>();
        AddSelf(this);
    }
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Player")) {
            Collected = true;
            _tvScript.CheckWin();
            gameObject.SetActive(false);
            _tvScript.CheckComplete();
        }
    }

    public void AddSelf(Bottles bottle) {
       _tvScript.BottlesList.Add(bottle);     
    }
}
