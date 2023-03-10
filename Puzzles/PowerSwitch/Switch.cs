using System;
using System.Collections;
using System.Collections.Generic;
using Highlighters;
using TMPro;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool IncludedInPuzzle;
    public bool Activated = false; //For later use
    public int CorrespondingNumber;
    public MeshRenderer CorrespondingLight;

    [SerializeField] private Material _redLight;
    [SerializeField] private Material _greenLight;
    
    [SerializeField] private Animator _anim;

    private void Awake() {
        _anim = GetComponent<Animator>();
        CorrespondingLight.material = _redLight;
    }

    public void Activate() {
        Activated = true;
        CorrespondingLight.material = _greenLight;
        _anim.SetBool("Activated", Activated);
        AudioManager.Instance.Play("SwitchSound1", transform.position);
    }

    public void Deactivate() {
        Activated = false;
        CorrespondingLight.material = _redLight;
        _anim.SetBool("Activated", Activated);
        AudioManager.Instance.Play("SwitchSound1", transform.position);
    }
    public void Outline()
    {
        HighlightManager.Instance.AddToOutline(gameObject);
    }
    public void StopOutline()
    {
        HighlightManager.Instance.RemoveFromOutline(gameObject);
    }
}
