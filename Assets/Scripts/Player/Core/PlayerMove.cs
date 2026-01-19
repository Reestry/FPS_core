using System;
using UnityEngine;

public enum MoveState { Walk, Sprint, Crouch }

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : PlayerInput, IInputable
{
    [SerializeField] private float _speed;
    [SerializeField] public GameObject _playerObj;
    [SerializeField] private float _itemPushForce = 10;

    private CharacterController _characterController;

    private Vector2 _movement;

    protected MoveState _moveState;
    
    private float _defaultSpeed = 5f;
    private float _sprintSpeed = 10f;
    private float _crouchSpeed = 2;

    public void SetState(MoveState state)
    {
        _moveState = state;
        
        switch (_moveState)
        {
        case MoveState.Walk:
            _speed = _defaultSpeed; break;
        case MoveState.Sprint:
            _speed = _sprintSpeed; break;
        case MoveState.Crouch:
            _speed = _crouchSpeed; break;
        }
    } 
    
    private void OnEnable()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler.SetInput(this);
        
        _inputHandler.OnMoveHandler += GetMove;
        
        SetState(MoveState.Walk);
        
    }

    private void Moving()
    {
        if (_inputHandler.ReturnHandler().Player.Sprint.IsPressed() && _characterController.isGrounded && _moveState == MoveState.Walk)
            SetState(MoveState.Sprint);
        if (_inputHandler.ReturnHandler().Player.Sprint.WasReleasedThisFrame() && _moveState == MoveState.Sprint)
            SetState(MoveState.Walk);

        var mov = (transform.right * _movement.x + transform.forward * _movement.y);
        _characterController.Move(mov * _speed * Time.deltaTime);
    }
    
    private void GetMove(Vector2 move)
    {
        _movement = move;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var obj = hit.collider.attachedRigidbody;

        if (obj == null || obj.isKinematic)
            return;

        obj.AddForce(hit.moveDirection * _itemPushForce * Time.deltaTime);
    }

    public void Run()
    {
        Moving();
    }


}