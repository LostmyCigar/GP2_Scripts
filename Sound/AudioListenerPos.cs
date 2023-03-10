using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerPos : MonoBehaviour
{
    [SerializeField] private Camera _camera1;
    [SerializeField] private Camera _camera2;
    [SerializeField] private float _smoothing = 0.1f;
    private Vector3 _targetPosition;
    

    private float _cameraDistance;

    private void Start() {
    }

    private void LateUpdate() {
        _cameraDistance = Vector3.Distance(_camera1.transform.position, _camera2.transform.position);

        float distanceToCamera1 = Vector3.Distance(transform.position, _camera1.transform.position);
        float distanceToCamera2 = Vector3.Distance(transform.position, _camera2.transform.position);

        if (distanceToCamera1 < distanceToCamera2)
        {
            _targetPosition = _camera1.transform.position + (transform.position - _camera1.transform.position).normalized * (_cameraDistance / 2f);
        }
        else
        {
            _targetPosition = _camera2.transform.position + (transform.position - _camera2.transform.position).normalized * (_cameraDistance / 2f);
        }

        // Smooth the position of the game object
        transform.position = Vector3.Lerp(transform.position, _targetPosition, _smoothing);
    }
}

