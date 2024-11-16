using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAction : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private LineRenderer _laserRenderer;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private WeaponAnimator _weaponAnimator;
    [SerializeField] private ParticleSystem _muzzleFlashParticleS;
    [SerializeField] private WeaponSoundManager _weaponSoundManager;

    public event Action OnAmmoLoaded;
    public event Action OnReloadEnded;
    public event Action OnFireEnded;
    public event Action OnSwitchEnded;

    private float _sway = 0f;
    private LayerMask _layerMask;
    private bool _playerAiming = false;
    private bool _playerMoving = false;
    private int _shotAccumulation = 0;
    private Vector2 _previousSpread = Vector2.zero;

    private List<Vector2> _spreadPattern = new List<Vector2>();

    private void Start()
    {
        _sway = GameManager.Instance.GetPlayerManager().Sway;
        _weaponManager.OnSpreadMaxChange += UpdateSpreadArrays;
        _weaponManager.OnSpreadAccumulationChange += UpdateSpreadArrays;

        UpdateSpreadArrays();
    }

    private void Update()
    {
        if (_shotAccumulation <= 0)
        {
            StopCoroutine("LooseSpreadAccumulation");
        }
    }

    private void UpdateSpreadArrays()
    {
        for (int i = 0; i < _weaponManager.SpreadMax + 1; i++)
        {
            _spreadPattern.Add(ApplyPatternSpread(i));
        }
    }

    private Vector2 GetBulletSpread()
    {
        Vector2 bulletSpread = _spreadPattern[_shotAccumulation];

        if (_playerMoving && !_playerAiming)
        {
            Vector2 newSpread;
            newSpread.x = bulletSpread.x > 0 ? bulletSpread.x + _sway : bulletSpread.x - _sway;
            newSpread.y = bulletSpread.y > 0 ? bulletSpread.y + _sway : bulletSpread.y - _sway;
            return newSpread;
        }
        else if (_playerMoving && _playerAiming)
        {
            Vector2 newSpread;
            newSpread.x = bulletSpread.x > 0 ? bulletSpread.x + _sway : bulletSpread.x - _sway;
            newSpread.y = bulletSpread.y > 0 ? bulletSpread.y + _sway : bulletSpread.y - _sway;
            return newSpread / 2;
        }
        else if (!_playerMoving && !_playerAiming)
        {
            return bulletSpread;
        }
        else
        {
            return bulletSpread / 2;
        }
    }

    public void Fire(LayerMask layer, bool moving, bool aiming)
    {
        if (_bulletSpawnPoint != null)
        {
            _layerMask = layer;
            _playerAiming = aiming;
            _playerMoving = moving;
            _weaponAnimator.FireWeaponAnimation();
            _weaponAnimator.OnFireAnimationTrigger += OnFireAnimationTrigger;
            _weaponAnimator.OnFireAnimationEnded += FireEnded;
        }
    }

    public void OnFireAnimationTrigger()
    {
        _weaponAnimator.OnFireAnimationTrigger -= OnFireAnimationTrigger;

        bool isHeadShot = _layerMask != LayerMask.NameToLayer("Enemy") ? CheckIfHeadShot() : false;
        if (_weaponManager.Bullet > 1)
        {
            _weaponSoundManager.PlayFireSound(_weaponManager.FireAudio);
            for (int i = 0; i < _weaponManager.Bullet; i++)
            {
                Vector2 spread = GetBulletSpread();
                WeaponFire(_layerMask.value, spread, isHeadShot);
            }

            _shotAccumulation++;

            if (_shotAccumulation == 1)
            {
                StartCoroutine("LooseSpreadAccumulation");
            }
            else if (_shotAccumulation > _weaponManager.SpreadMax)
            {
                _shotAccumulation = _weaponManager.SpreadMax;
            }

            _weaponManager.UpdateMag(-1);
        }
        else
        {
            Vector2 spread = GetBulletSpread();
            _shotAccumulation++;

            if (_shotAccumulation == 1)
            {
                StartCoroutine("LooseSpreadAccumulation");
            }
            else if (_shotAccumulation > _weaponManager.SpreadMax)
            {
                _shotAccumulation = _weaponManager.SpreadMax;
            }

            WeaponFire(_layerMask.value, spread, isHeadShot);
            _weaponSoundManager.PlayFireSound(_weaponManager.FireAudio);
            _weaponManager.UpdateMag(-1);
        }
        _muzzleFlashParticleS.Emit(10);
    }

    private Vector2 ApplyPatternSpread(float accumulation)
    {
        Vector2 newMinSpread = _previousSpread;
        float newMaxSpread = _weaponManager.SpreadAccumulation + (accumulation / 10);

        float positifX = UnityEngine.Random.Range(newMinSpread.x, newMaxSpread);
        float negatifX = UnityEngine.Random.Range(-newMinSpread.x, -newMaxSpread);
        float positifY = UnityEngine.Random.Range(newMinSpread.y, newMaxSpread);
        float negatifY = UnityEngine.Random.Range(-newMinSpread.y, -newMaxSpread);

        Vector2 newSpread;
        if (UnityEngine.Random.Range(1, 3) % 2 == 0)
        {
            newSpread.x = positifX;
        }
        else
        {
            newSpread.x = negatifX;
        }

        if (UnityEngine.Random.Range(1, 3) % 2 == 0)
        {
            newSpread.y = positifY;
        }
        else
        {
            newSpread.y = negatifY;
        }

        _previousSpread = newSpread;
        return newSpread;
    }

    private IEnumerator LooseSpreadAccumulation()
    {
        while (_shotAccumulation > 0)
        {
            yield return new WaitForSeconds(0.4f);
            _shotAccumulation -= 1;
        }
    }

    private void WeaponFire(int layer, Vector2 spread, bool isHeadShot)
    {
        Vector3 defaultBulletSpawnPoint = _bulletSpawnPoint.transform.rotation.eulerAngles;

        Ray ray = new Ray(_bulletSpawnPoint.transform.position, _bulletSpawnPoint.transform.forward);
        Vector3 target = ray.GetPoint(_weaponManager.Range);
        target += new Vector3(spread.x, spread.y, 0);

        _bulletSpawnPoint.transform.LookAt(target);
        GameObject bulletGo = GameData.Instance.BulletPool.GetGameObject(_bulletSpawnPoint.transform.position, _bulletSpawnPoint.rotation);
        if (bulletGo != null && bulletGo.TryGetComponent(out Bullet bullet))
        {
            bulletGo.SetActive(true);
            bullet.IsHeadShot = isHeadShot;
            bullet.StunTimer = _weaponManager.StunTimer;
            bullet.LayerToIgnore.value = layer;
            bullet.BulletDamage = isHeadShot ? _weaponManager.Damage * _weaponManager.HeadShotDamageMultiplier : _weaponManager.Damage;
            _bulletSpawnPoint.transform.rotation = Quaternion.Euler(defaultBulletSpawnPoint);
        }
    }

    private bool CheckIfHeadShot()
    {
        int random = UnityEngine.Random.Range(0, 100);
        return random <= _weaponManager.HeadShotChance;
    }

    private void FireEnded()
    {
        _weaponAnimator.OnFireAnimationEnded -= FireEnded;
        OnFireEnded?.Invoke();
    }

    public void Reload()
    {
        _weaponAnimator.ReloadAnimation();
        _weaponAnimator.OnReloadAnimationEnded += ReloadEnded;
    }

    private void ReloadEnded()
    {
        _weaponManager.ReinitializeMag();
        _weaponAnimator.OnReloadAnimationEnded -= ReloadEnded;
        OnReloadEnded?.Invoke();
    }

    private void IncrementalReload()
    {
        if (_weaponManager.CurrentMag < _weaponManager.MagSize)
        {
            _weaponManager.UpdateMag(1);
            OnAmmoLoaded?.Invoke();
        }

        if (_weaponManager.CurrentMag == _weaponManager.MagSize)
        {
            StopReload();
        }
    }

    public void StartContinuousReload()
    {
        _weaponAnimator.ContinuousReloadAnimation(true);
        _weaponAnimator.OnContinuousReloadAnimationEnded += IncrementalReload;
        _weaponAnimator.OnTransitionToIdleAnimationEnded += TransitionToIdleEnded;
    }

    public void StopReload()
    {
        _weaponAnimator.ContinuousReloadAnimation(false);
        _weaponAnimator.OnContinuousReloadAnimationEnded -= IncrementalReload;
    }

    public void TransitionToIdleEnded()
    {
        _weaponAnimator.OnTransitionToIdleAnimationEnded -= TransitionToIdleEnded;
        OnReloadEnded?.Invoke();
    }

    public void SwitchWeapon()
    {
        _weaponAnimator.SwitchWeaponAnimation();
        _weaponAnimator.OnSwitchWeaponAnimationTrigger += SwitchWeaponAnimationTrigger;
        _weaponAnimator.OnSwitchWeaponAnimationEnded += SwitchWeaponAnimationEnded;
    }

    private void SwitchWeaponAnimationTrigger()
    {
        _weaponAnimator.OnSwitchWeaponAnimationTrigger -= SwitchWeaponAnimationTrigger;
        _weaponManager.ChangeWeaponVisibility(!_weaponManager.IsRendered);
    }

    private void SwitchWeaponAnimationEnded()
    {
        _weaponAnimator.OnSwitchWeaponAnimationEnded -= SwitchWeaponAnimationEnded;
        OnSwitchEnded?.Invoke();
    }
}
