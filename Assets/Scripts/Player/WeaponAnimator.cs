using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponAnimator : MonoBehaviour
{


    private InputHandler _inputHandler;

    private Animator _animator;

    private bool _isAnimating;
    private readonly int IsInteract = Animator.StringToHash("IsInteract");
    private readonly int IsReload = Animator.StringToHash("IsReload");

    private Vector3 _startPos;

    
    
    private void OnEnable()
    {
        _startPos = transform.localPosition;

    }

    private void Start()
    {
        _inputHandler = GetComponentInParent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();
        
        //_inputHandler.OnReloadPressed += Reload;
        _inputHandler.OnInteractPresssed += Interact;

    }
 
    private void Update()
    {

    }

    public void ResetAnimating()
    {
        _isAnimating = false;
    }

    private Tween _shootTween;

    private float _shootAmplitude = 0.1f;
    public void Shoot(float rate, bool isAiming)
    {
        _shootTween?.Kill();
        var currentPos = transform.localPosition;
        var currentRot = transform.localRotation.eulerAngles;


        _shootAmplitude = isAiming ? 0.01f : 0.1f;
        
        _shootTween = DOTween.Sequence()
            
            .Append(transform.DOLocalMoveZ(currentPos.z - _shootAmplitude, rate * 0.25f)
                .SetEase(Ease.OutQuad))
            .Join(transform.DOLocalRotate(new Vector3(currentRot.x - 5f, currentRot.y, currentRot.z), rate * 0.25f)
                .SetEase(Ease.OutQuad))
            
            .Append(transform.DOLocalMove(currentPos, rate * 0.75f)
                .SetEase(Ease.InQuad))
            .Join(transform.DOLocalRotate(currentRot, rate * 0.75f)
                .SetEase(Ease.InQuad))
            .SetAutoKill(false);
    }
    



    public void ReleasePosition()
    {
        /*transform.localPosition = _startPos;*/
    }

    public void Aim(float _aimFov, Transform _aimPos)
    {
        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, _aimFov, 0.1f)
            .SetEase(Ease.OutSine).SetAutoKill(true);

        DOTween.Sequence()
            .Append(
                transform.DOLocalMove(_aimPos.localPosition, 0.2f).SetEase(Ease.OutElastic, 1f, 0.3f))
            .Join(transform.DOLocalRotate(_aimPos.localRotation.eulerAngles, 0.2f)
                .SetEase(Ease.OutElastic, 1f, 0.3f))
            .SetAutoKill(true);
    }

    public void ResetAim(float _defaultFov, Vector3 _currentPos, Quaternion _currentRot)
    {
        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, _defaultFov, 0.1f)
            .SetEase(Ease.InSine).SetAutoKill(true);

        DOTween.Sequence()
            .Append(transform.DOLocalMove(_currentPos, 0.2f).SetEase(Ease.OutElastic, 1f, 0.3f))
            .Join(transform.DOLocalRotate(_currentRot.eulerAngles, 0.2f)
                .SetEase(Ease.OutElastic, 1, 0.3f))
            .SetAutoKill(true);
    }


    private float _crouchPos = 0.1f;
    public void CrouchAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOLocalMoveX(-_crouchPos, 0.5f).SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalMoveY(-_crouchPos, 0.5f).SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalRotate(new Vector3(transform.localRotation.x, transform.localRotation.y,
                transform.localRotation.z + _crouchPos ), 0.5f). SetEase(Ease.OutElastic, 1.5f, 0.2f))
            .SetAutoKill(true);
    }
    
    public void ResetCrouchAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOLocalMoveX(+_crouchPos, 0.5f).SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalMoveY(+_crouchPos, 0.5f).SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalRotate(new Vector3(transform.localRotation.x, transform.localRotation.y,
                transform.localRotation.z - _crouchPos ), 0.5f). SetEase(Ease.OutElastic, 1.5f, 0.2f))
            .SetAutoKill(true);
    }
    
    public void Reload()
    {
        _animator.SetTrigger(IsReload);
    }

    private void Interact()
    {
        _animator.SetTrigger(IsInteract); 
    }
}
