using System;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerShoot : MonoBehaviour, IInputable
{
    
    // TODO: make it with enums
    private bool IsAiming;

    [SerializeField] private WeaponObject _weaponObject;

    [SerializeField] private GameObject _weaponObj;
    [SerializeField] private Transform _aimPos;
    [SerializeField] private GameObject _cameraHolder;

    private InputHandler _inputHandler;
    private WeaponAnimator _weaponAnimator;

    private Vector3 _currentPos;
    private Quaternion _currentRot;

    private float _aimFov;
    private float _defaultFov;

    private float _timeRate;

    private int _currentAmmo;

    private Tween _cameraTween;
    private Vector3 _startRot;

    private float _currentSpread;

    // weapon stats

    private float _shootRate;

    private float _recoilForce;
    private int _ammoCount;

    private float _reloadTime;
    private GameObject _reloadedClip;

    // TODO: To recoil script
    private float _recoilDuration;
    private float _recoilResetSpeed;
    private float _firstResetSpeed;

    // TODO: To spread script
    private float _maxSpread;
    private float _spreadPerShot;
    private float _spreadRecovery;

    //
    private float _shootAmplitude;


    private Transform _startPos;

    //Set Weapon Scriptable Object
    private void SetWeaponStats(WeaponObject weapon)
    {
        // Can be deleted
        _currentAmmo = weapon.AmmoCount;

        _ammoCount = weapon.AmmoCount;
        _reloadTime = weapon.ReloadTime;
        _reloadedClip = weapon.ReloadedClip;
        _shootRate = weapon.ShootRate;
        _recoilForce = weapon.RecoilForce;
        _maxSpread = weapon.MaxSpread;
        _spreadPerShot = weapon.SpreadPerShot;
        _spreadRecovery = weapon.SpreadRecovery;
        _recoilDuration = weapon.RecoilDuration;
        _recoilResetSpeed = weapon.RecoilResetSpeed;
        _firstResetSpeed = weapon.FirstResetSpeed;
        _shootAmplitude = weapon.ShootAmplitude;
    }

    private void OnEnable()
    {
        _inputHandler = GetComponent<InputHandler>();
        _inputHandler.SetInput(this);
        _weaponAnimator = GetComponentInChildren<WeaponAnimator>();

        SetWeaponStats(_weaponObject);
        _defaultFov = Camera.main.fieldOfView;
        _aimFov = _defaultFov / _weaponObject.AimFovMultiplier;
        _weaponAnimator.SetValues(_shootAmplitude, _defaultFov, _aimFov);

        _inputHandler.OnReloadPressed += Reload;

        _currentPos = _weaponObj.transform.localPosition;
        _currentRot = _weaponObj.transform.localRotation;

        _startPos = _weaponObj.transform;

        _startRot = _cameraHolder.transform.localRotation.eulerAngles;
    }

    private async void Reload()
    {
        if (_currentAmmo == _ammoCount || IsAiming)
            return;
        
        
        _weaponAnimator.Reload();

        await Task.Delay((int) (_reloadTime * 1000));
        Instantiate(_reloadedClip, new Vector3(transform.position.x, transform.position.y, transform.position.z),
            quaternion.Euler(50, 30, 0));
        _currentAmmo = _ammoCount;
    }

    private void Shoot()
    {
        if (_weaponAnimator == null)
            return;

        _currentAmmo--;
        _weaponAnimator.Shoot(_shootRate, IsAiming);

        CameraRecoil(_recoilDuration, _recoilResetSpeed);

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        var shootDir = ray.direction;

        _currentSpread += _spreadPerShot;

        _currentSpread = Mathf.Clamp(_currentSpread, 0f, _maxSpread);

        // Разброс
        if (!IsAiming)
            shootDir = Quaternion.Euler(Random.Range(-_currentSpread, _currentSpread),
                           Random.Range(-_currentSpread, _currentSpread), 0)
                       * shootDir;

        if (Physics.Raycast(ray.origin, shootDir, out var hit))
        {
            if (hit.collider == null)
                return;

            var rbObj = hit.collider.attachedRigidbody;
            if (rbObj != null)
                hit.collider.attachedRigidbody.AddForce(shootDir * 50, ForceMode.Impulse);

            Debug.Log(hit.collider.name);

            //Destroy(hit.collider.gameObject);
        }
    }

    private void CameraRecoil(float recoilDuration, float resetSpeed)
    {
        _cameraTween?.Kill();
        var currentRot = _cameraHolder.transform.localRotation.eulerAngles;

        var recoilX = Random.Range(0f, -2f);
        var recoilY = Random.Range(-1f, 1f);

        _cameraTween = DOTween.Sequence()
            .Append(_cameraHolder.transform.DOLocalRotate(
                new Vector3(currentRot.x + recoilX * _recoilForce, currentRot.y + recoilY * _recoilForce, currentRot.z),
                recoilDuration).SetEase(Ease.OutQuad))
            .Append(_cameraHolder.transform.DOLocalRotate(currentRot, _firstResetSpeed)
                .SetEase(Ease.InQuad))
            .Append(_cameraHolder.transform.DOLocalRotate(_startRot, resetSpeed).SetEase(Ease.OutQuad))
            .SetAutoKill(false);
    }

    public void Run()
    {
        if (_inputHandler.ReturnHandler().Player.Attack.IsPressed() && CanShoot())
            Shoot();

        if (_inputHandler.ReturnHandler().Player.Aim.WasPressedThisFrame())
        {
            IsAiming = true;
            _weaponAnimator.Aim(_aimPos);
        }

        if (_inputHandler.ReturnHandler().Player.Aim.WasReleasedThisFrame())
        {
            IsAiming = false;

            _weaponAnimator.ResetAim(_currentPos, _currentRot);
        }

        _currentSpread = Mathf.MoveTowards(_currentSpread, 0f, _spreadRecovery * Time.deltaTime);
    }

    private bool CanShoot()
    {
        if (_currentAmmo == 0)
            return false;

        if (!(Time.time > _timeRate))
            return false;

        _timeRate = Time.time + _shootRate;
        return true;
    }
}