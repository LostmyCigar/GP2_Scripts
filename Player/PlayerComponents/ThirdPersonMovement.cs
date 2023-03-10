using System;
using System.Net.Sockets;
using UnityEngine;

public class ThirdPersonMovement : PlayerComponent
{
    private Camera _camera;
    private InputHandler _inputHandler;
    private GhostData _ghostData;
    private Rigidbody _rb;
    private Player _player;
    private PlayerType _type;
    public bool _canMove;
    private Animator _animator;

    private Vector3 _targetDir;
    private Action Move;
    
    public ThirdPersonMovement(Player player, Camera camera, InputHandler inputHandler, GhostData ghostData, Rigidbody rb, PlayerType type, Animator animator) {
        _animator = animator;
        _player = player;
        _camera = camera;
        _inputHandler = inputHandler;
        _ghostData = ghostData;
        _rb = rb;
        _canMove = true;
        _type = type;
    }

    public override void PhysicsUpdate() {

        if (!_canMove)
            return;

        if (_player.CurrentGameState == GameState.NormalState) {
            HandleMovement();
            HandleRotation();
        }
        else if (_rb.velocity != Vector3.zero) {
            StopMovement();
        }
    }

    public void StopMovement() {
        _rb.velocity = Vector3.zero;
        //_rb.isKinematic = false;
    }

    private void HandleMovement() {
        if (_type == PlayerType.Human)
            HumanMovement();
        else if (_type == PlayerType.Ghost)
            DogMovement();  
    }


    private void HumanMovement()
    {
        Vector2 inputVector = new Vector2(_inputHandler._moveDir.z, _inputHandler._moveDir.x);

        Vector3 moveDir = _camera.transform.forward * inputVector.x;
        moveDir += _camera.transform.right * inputVector.y;
        moveDir.y = 0;
        moveDir.Normalize();
        moveDir *= _ghostData.MoveSpeed;
        moveDir.y = _rb.velocity.y;

        if (inputVector.magnitude == 0)
        {
            _rb.velocity /= _ghostData.Deaccel;
        }
        else
        {
            _rb.velocity = moveDir;
            AudioManager.Instance.Play("ZhiFootstep", _player.transform.position);
        }
    }

    private void DogMovement()
    {
        int input = 0;
        if (_inputHandler._moveDir.magnitude != 0)
            input = 1;

        // float xValue = Mathf.InverseLerp(-1, 1, _inputHandler._moveDir.x);
        // float zValue = Mathf.InverseLerp(-1, 1, _inputHandler._moveDir.z);
        
        _targetDir *= input;
        _animator.SetFloat("Velocity X", _inputHandler._moveDir.x);
        _animator.SetFloat("Velocity Y", _inputHandler._moveDir.z);
        _rb.velocity = new Vector3(_targetDir.x * _ghostData.MoveSpeed, _rb.velocity.y, _targetDir.z * _ghostData.MoveSpeed);
        
        if (_rb.velocity != Vector3.zero) {
            AudioManager.Instance.Play("DogFootstep", _player.transform.position);
        }
    }


    private void HandleRotation() {
        if (_type == PlayerType.Human)
            HumanRotation();
        else if (_type == PlayerType.Ghost)
            DogRotation();
    }

    private void HumanRotation()
    {
        Vector3 targetDir;

        targetDir = _camera.transform.forward * _inputHandler._moveDir.z;
        targetDir += _camera.transform.right * _inputHandler._moveDir.x;
        targetDir.y = 0;
        
        _animator.SetFloat("Velocity X", _inputHandler._moveDir.x);
        _animator.SetFloat("Velocity Y", _inputHandler._moveDir.z);

        if (targetDir == Vector3.zero)
        {
            targetDir = _rb.transform.forward;
        }

        Quaternion tr = Quaternion.LookRotation(targetDir);
        _rb.transform.rotation =
            Quaternion.Slerp(_rb.transform.rotation, tr, _ghostData.RotationSpeed * Time.deltaTime);
    }

    private void DogRotation()
    {
        var targetDir = _camera.transform.forward * _inputHandler._moveDir.z;
        targetDir += _camera.transform.right * _inputHandler._moveDir.x;
        targetDir.y = 0;


        if (targetDir == Vector3.zero) targetDir = _rb.transform.forward;

        var tr = Quaternion.LookRotation(targetDir);
        _rb.transform.rotation = Quaternion.Slerp(_rb.transform.rotation, tr, _ghostData.RotationSpeed * Time.deltaTime); //hardcoded rotation spped fix later

        _targetDir = _rb.transform.forward;
    }
}