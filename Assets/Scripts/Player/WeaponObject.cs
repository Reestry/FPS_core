using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items / Weapon")]
public class WeaponObject : ScriptableObject
{
    [Header(" Weapon Stats")] [SerializeField]
    private int _ammoCount = 30;

    [Range(0f, 10f)] [SerializeField] private float _reloadTime = 1.5f;
    [SerializeField] private GameObject _reloadedClip;

    [SerializeField] private float _shootRate = 0.2f;

    [Range(0.5f, 5)] [SerializeField] private float _recoilForce = 1f;

    [Header("\n Spread")] [SerializeField] private float _maxSpread = 5f;
    [SerializeField] private float _spreadPerShot = 0.5f;
    [SerializeField] private float _spreadRecovery = 2f;

    [Header("\n Recoil")] [SerializeField] private float _recoilDuration = 0.12f;
    [SerializeField] private float _recoilResetSpeed = 0.1f;
    [SerializeField] private float _firstResetSpeed = 0.14f;
    public int AmmoCount => _ammoCount;
    public float ReloadTime => _reloadTime;
    public GameObject ReloadedClip => _reloadedClip;

    public float ShootRate => _shootRate;
    public float RecoilForce => _recoilForce;

    public float MaxSpread => _maxSpread;
    public float SpreadPerShot => _spreadPerShot;
    public float SpreadRecovery => _spreadRecovery;

    public float RecoilDuration => _recoilDuration;
    public float RecoilResetSpeed => _recoilResetSpeed;
    public float FirstResetSpeed => _firstResetSpeed;
}