using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    private void OnEnable() {
        //AudioManager.Instance.Play("MainTheme", transform.position);
        StartCoroutine(PlayMusic());
    }

    private IEnumerator PlayMusic() {
        yield return new WaitForSeconds(3.5f);
        AudioManager.Instance.Play("MainTheme", transform.position);
            
    }
}
