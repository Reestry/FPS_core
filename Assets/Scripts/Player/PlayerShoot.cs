using System;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerShoot : MonoBehaviour, IInputable
{
    [SerializeField] private GameObject _weaponObj;
    [SerializeField] private Transform _aimPos;

    public bool IsAiming;

    private InputHandler _inputHandler;
    private WeaponAnimator _weapon;

    private Vector3 _currentPos;
    private Quaternion _currentRot;

    private float _aimFov;
    private float _defaultFov;

    private float _timeRate;

    private int _currentAmmo;

    // weapon stats
    [Header(" Weapon Stats (delete later)")] [SerializeField]
    private float _shootRate = 0.2f;

    [Range(0.5f, 5)] [SerializeField] private float _recoilForce = 1f;
    [SerializeField] private int _ammoCount = 30;
    
    
    [Range(0f, 10f)] [SerializeField] private float _reloadTime = 1.5f;
    [SerializeField] private GameObject _reloadedClip;
    
    [Header("\n Recoil")] [SerializeField] private float _recoilDuration = 0.12f;
    [SerializeField] private float _recoilResetSpeed = 0.1f;
    [SerializeField] private float _firstableResetSpeed = 0.14f;
    [SerializeField] private GameObject _cameraHolder;
    private Tween _cameraTween;
    private Vector3 _startRot;

    private float _currentSpread;

    [Header("\n Spread")] [SerializeField] private float _maxSpread = 5f;
    [SerializeField] private float _spreadPerShot = 0.5f;
    [SerializeField] private float _spreadRecovery = 2f;

    //Set Weapon Scriptable Object
    private void SetWeaponStats()
    {
        _currentAmmo = _ammoCount;
    }

    private void OnEnable()
    {
        _inputHandler = GetComponent<InputHandler>();
        _inputHandler.SetInput(this);
        _weapon = GetComponentInChildren<WeaponAnimator>();

        _inputHandler.OnReloadPressed += Reload;

        _defaultFov = Camera.main.fieldOfView;
        _aimFov = _defaultFov / 1.5f;

        _currentPos = _weaponObj.transform.localPosition;
        _currentRot = _weaponObj.transform.localRotation;

        _startRot = _cameraHolder.transform.localRotation.eulerAngles;

        SetWeaponStats();
    }

    private async void Reload()
    {
        if (_currentAmmo == _ammoCount)
            return;
        
        _weapon.Reload();
        
        await Task.Delay((int) (_reloadTime * 1000));
        Instantiate(_reloadedClip, new Vector3(transform.position.x, transform.position.y, transform.position.z), quaternion.Euler(50,30,0));
        _currentAmmo = _ammoCount;
    }

    private void Shoot()
    {
        if (_weapon == null)
            return;

        _currentAmmo--;
        _weapon.Shoot(_shootRate, IsAiming);

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
            .Append(_cameraHolder.transform.DOLocalRotate(currentRot, _firstableResetSpeed)
                .SetEase(Ease.InQuad))
            .Append(_cameraHolder.transform.DOLocalRotate(_startRot, resetSpeed).SetEase(Ease.OutQuad))
            .SetAutoKill(false);
    }

    public void Run()
    {
        if (_inputHandler.ReturnHandler().Player.Attack.IsPressed() && CanShoot())
            Shoot();

        if (_inputHandler.ReturnHandler().Player.Attack.WasReleasedThisFrame())
        {
            _weapon.ReleasePosition();
        }

        if (_inputHandler.ReturnHandler().Player.Aim.WasPressedThisFrame())
        {
            IsAiming = true;
            _weapon.Aim(_aimFov, _aimPos);
        }

        if (_inputHandler.ReturnHandler().Player.Aim.WasReleasedThisFrame())
        {
            IsAiming = false;
            _weapon.ResetAim(_defaultFov, _currentPos, _currentRot);
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