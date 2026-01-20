using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMove))]
public class PlayerCrouch : PlayerInput, IInputable
{
    
    
    private CharacterController _characterController;
    private Vector3 _originalScale;
    private Vector3 _crouchScale;
    private float _currentHeight;
    private float _crouchHeinght;


    private PlayerMove _playerMove;
    private WeaponAnimator _weaponAnimator;
    private void OnEnable()
    {
        _playerMove = GetComponent<PlayerMove>();
        _characterController = GetComponent<CharacterController>();
        _weaponAnimator = GetComponentInChildren<WeaponAnimator>();
        _inputHandler.SetInput(this);

        var height = _characterController.height;
        _currentHeight = height;
        _crouchHeinght = height / 2;

        _originalScale = _playerMove._playerObj.transform.localScale;
        var scale = _originalScale;
        scale.y /= 2;
        _crouchScale = scale;
    }

    public void Run()
    {
        var crouchAction = _inputHandler.ReturnHandler().Player.Crouch;

        if (crouchAction.WasPressedThisFrame())
        {
            _playerMove._playerObj.transform.localScale = _crouchScale;
            _characterController.height = _crouchHeinght;

            _playerMove.SetState(MoveState.Crouch);
            
            _weaponAnimator.CrouchAnimation();
        }

        if (_inputHandler.ReturnHandler().Player.Crouch.WasReleasedThisFrame()) 
        {
            _playerMove._playerObj.transform.localScale = _originalScale;
            _characterController.height = _currentHeight;
            
            _playerMove.SetState(MoveState.Walk); 
            
            _weaponAnimator.ResetCrouchAnimation();
        }
    }
}