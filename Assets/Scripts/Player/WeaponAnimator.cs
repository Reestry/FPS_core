using DG.Tweening;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    private InputHandler _inputHandler;

    private Animator _animator;

    private bool _isAnimating;
    private readonly int IsInteract = Animator.StringToHash("IsInteract");
    private readonly int IsReload = Animator.StringToHash("IsReload");

    private Vector3 _standPos;
    private Quaternion _standRot;
    private Vector3 _crouchWeaponPos;
    private Quaternion _crouchWeaponRot;

    private bool _isCrouching;
    private Tween _shootTween;
    private Tween _crouchTween;

    private float _shootAmplitude;
    private float _aimFov;
    private float _defaultFov;

    private void OnEnable()
    {
        _standPos = transform.localPosition;
        _standRot = transform.localRotation;
        _crouchWeaponPos = _standPos + new Vector3(-0.4f, -0.1f, 0f);
        _crouchWeaponRot = _standRot * Quaternion.Euler(0, 0, 15f);
    }

    public void SetValues(float shootAmplitude, float defaultFov, float aimFov)
    {
        _shootAmplitude = shootAmplitude;
        _defaultFov = defaultFov;
        _aimFov = aimFov;
    }

    private void Start()
    {
        _inputHandler = GetComponentInParent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();

        _inputHandler.OnInteractPresssed += Interact;
    }

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

    // TODO: Aim & ResetAim need to unite in 1 void
    public void Aim(Transform _aimPos)
    {
        _crouchTween?.Kill();

        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, _aimFov, 0.1f)
            .SetEase(Ease.OutSine).SetAutoKill(true);

        DOTween.Sequence()
            .Append(
                transform.DOLocalMove(_aimPos.localPosition, 0.2f).SetEase(Ease.OutElastic, 1f, 0.3f))
            .Join(transform.DOLocalRotate(_aimPos.localRotation.eulerAngles, 0.2f)
                .SetEase(Ease.OutElastic, 1f, 0.3f))
            .SetAutoKill(true);
    }

    public void ResetAim(Vector3 _defaultPos, Quaternion _defaultRot)
    {
        _crouchTween?.Kill();

        if (_isCrouching)
        {
            _defaultPos = _crouchWeaponPos;
            _defaultRot = _crouchWeaponRot;
        }

        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, _defaultFov, 0.1f)
            .SetEase(Ease.InSine).SetAutoKill(true);

        DOTween.Sequence()
            .Append(transform.DOLocalMove(_defaultPos, 0.2f).SetEase(Ease.OutElastic, 1f, 0.3f))
            .Join(transform.DOLocalRotate(_defaultRot.eulerAngles, 0.2f)
                .SetEase(Ease.OutElastic, 1, 0.3f))
            .SetAutoKill(true);
    }

    public void CrouchAnimation()
    {
        _isCrouching = true;
        _crouchTween = DOTween.Sequence()
            .Join(transform.DOLocalMove(_crouchWeaponPos, 0.5f)
                .SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalRotate(_crouchWeaponRot.eulerAngles, 0.5f).SetEase(Ease.OutElastic, 1.5f, 0.2f))
            .SetAutoKill(true);
    }

    public void ResetCrouchAnimation()
    {
        _isCrouching = false;
        _crouchTween = DOTween.Sequence()
            .Append(transform.DOLocalMove(_standPos, 0.5f)
                .SetEase(Ease.OutElastic, 1, 0.3f))
            .Join(transform.DOLocalRotate(_standRot.eulerAngles, 0.5f)
                .SetEase(Ease.OutElastic, 1.5f, 0.2f))
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