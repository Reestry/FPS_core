using System;
using UnityEngine;

public class WeaponDuration : MonoBehaviour
{
    [SerializeField] private float _duration = 3;

    private float _time;
    public void SetValues(float duration)
    {
        _duration = duration;
    }

    public bool CanShoot()
    {
        _time += Time.deltaTime;

        if (_time >= _duration)
        {
            _time = 0;

            return true;
        }

        return false;
    }
    
}
