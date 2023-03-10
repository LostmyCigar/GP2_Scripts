using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _cameraFollowSpeed;


    private void Start()
    {
        if (_target == null)
            Debug.Log("Missing camera target. Assign in inspector");
    }
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _cameraFollowSpeed * Time.deltaTime); //illegal lerps :)
    }
}
