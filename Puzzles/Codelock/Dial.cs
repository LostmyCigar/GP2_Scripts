using System;
using UnityEngine;

public class Dial : MonoBehaviour {

    private LayerMask _layer;
    private void Awake() {
        _layer = gameObject.layer;
    }

    public void Outline() {
        gameObject.layer = 13;
    }
    public void StopOutline() {
        gameObject.layer = _layer;
    }
}
