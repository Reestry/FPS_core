using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class WeaponSway : MonoBehaviour
{
    //[SerializeField] private GameObject _weapon;

    [SerializeField] private float _swayMultiplier;
    [SerializeField] private float _smooth;

    [SerializeField] private float _position = 0.01f;
    [SerializeField] private float _duration = 0.01f;
    private InputHandler _playerInput;

    private Vector2 _rotation;

    private Vector3 _initPos;

    private void Start()
    {
        _playerInput = GetComponentInParent<InputHandler>();
        _initPos = transform.localPosition;
        IdleAnimation();

        _playerInput.OnMoveStarted += StartWalkAnimation;
        _playerInput.OnMoveEnded += IdleAnimation;
    }

    private void Update()
    {
        Sway();
    }

    private void IdleAnimation()
    {
        /*
        KillCurrentTween();
        transform.DOLocalMove(_initPos, 0.2f).SetEase(Ease.InElastic).SetAutoKill(true);
        var randomPos = Random.Range(-1, 2); 
        _currentTween = DOTween.Sequence() 
            .Append(transform.DOLocalMoveY(_initPos.y + _position, _duration) 
                .SetLoops(-1, LoopType.Yoyo) .SetEase(Ease.InOutSine)) 
            .Join(
                transform.DOLocalMoveX(_initPos.x + (_position * randomPos), _duration) 
                .SetLoops(-1, LoopType.Yoyo) .SetEase(Ease.InOutSine)
                )
            .SetAutoKill(true);
            */
    }

    private void Sway()
    {
        _rotation = _playerInput.ReturnHandler().Player.Look.ReadValue<Vector2>() * _swayMultiplier;

        var rotationY = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var rotationX = Quaternion.AngleAxis(-_rotation.y, Vector3.right);
        var target = rotationX * rotationY;

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation, target, _smooth * Time.deltaTime);
    }

    private Tween _currentTween;

    private void KillCurrentTween()
    {
        if (_currentTween != null)
            _currentTween.Kill(true);

        
    }

    private void StartWalkAnimation()
    {
        KillCurrentTween();
        /*
        Debug.Log("иду");
        _currentTween = DOTween.Sequence().Append(transform.DOLocalMoveY(_initPos.y + _position, 0.5f).SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)).SetAutoKill(true);
            */
    }
}