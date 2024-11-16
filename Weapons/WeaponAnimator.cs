using System;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 _aimingOffset;
    [SerializeField] private WeaponManager _weaponManager;

    public event Action OnFireAnimationEnded;
    public event Action OnFireAnimationTrigger;
    public event Action OnReloadAnimationEnded;
    public event Action OnSwitchWeaponAnimationEnded;
    public event Action OnSwitchWeaponAnimationTrigger;
    public event Action OnContinuousReloadAnimationEnded;
    public event Action OnTransitionToIdleAnimationEnded;
    public event Action OnTransitionToReloadAnimationEnded;

    private Transform weaponTransform;
    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
        weaponTransform = _weaponManager.gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (_playerManager.IsAiming && _weaponManager.IsOwnedByPlayer && weaponTransform.localPosition != _aimingOffset)
        {
            SetAimingPosition(true);
        }
        else if (!_playerManager.IsAiming && _weaponManager.IsOwnedByPlayer && weaponTransform.localPosition != Vector3.zero)
        {
            SetAimingPosition(false);
        }
    }

    private void SetAimingPosition(bool aiming)
    {
        Vector3 adjustPosition = Vector3.zero;
        if (aiming)
        {
            weaponTransform.Translate(_aimingOffset * Time.fixedDeltaTime * _weaponManager.AimingSpeed);
            adjustPosition = weaponTransform.localPosition;
            if (weaponTransform.localPosition.y > _aimingOffset.y) adjustPosition.y = _aimingOffset.y;
            if (weaponTransform.localPosition.x > _aimingOffset.x) adjustPosition.x = _aimingOffset.x;
            if (weaponTransform.localPosition.z > _aimingOffset.z) adjustPosition.z = _aimingOffset.z;
            if (adjustPosition != weaponTransform.localPosition) weaponTransform.localPosition = adjustPosition;
        }
        else
        {
            weaponTransform.Translate(-_aimingOffset * Time.fixedDeltaTime * _weaponManager.AimingSpeed);
            if (weaponTransform.localPosition.y < 0) weaponTransform.localPosition = adjustPosition;
            if (weaponTransform.localPosition.x < 0) weaponTransform.localPosition = adjustPosition;
            if (weaponTransform.localPosition.z < 0) weaponTransform.localPosition = adjustPosition;
        }
    }

    public void SetAnimationModifiers(float fireRate, float reloadRate)
    {
        _animator.SetFloat("FireRate", 2f - fireRate);
        _animator.SetFloat("ReloadRate", 2f - reloadRate);
    }

    public void FireWeaponAnimation()
    {
        _animator.SetTrigger("Fire");
    }

    public void ReloadAnimation()
    {
        _animator.SetTrigger("Reload");
    }

    public void ContinuousReloadAnimation(bool reload)
    {
        _animator.SetBool("ContinuousReload", reload);
    }

    public void SwitchWeaponAnimation()
    {
        _animator.SetTrigger("SwitchWeapon");
    }

    public void FireAnimationEnded()
    {
        OnFireAnimationEnded?.Invoke();
    }

    public void FireAnimationTrigger()
    {
        OnFireAnimationTrigger?.Invoke();
    }

    public void ReloadAnimationEnded()
    {
        OnReloadAnimationEnded?.Invoke();
    }

    public void ContinuousReloadAnimationEnded()
    {
        OnContinuousReloadAnimationEnded?.Invoke();
    }

    public void SwitchWeaponAnimationTrigger()
    {
        OnSwitchWeaponAnimationTrigger?.Invoke();
    }

    public void SwitchWeaponAnimationEnded()
    {
        OnSwitchWeaponAnimationEnded?.Invoke();
    }

    public void TransitionToReloadAnimationEnded()
    {
        OnTransitionToReloadAnimationEnded?.Invoke();
    }

    public void TransitionToIdleAnimationEnded()
    {
        OnTransitionToIdleAnimationEnded?.Invoke();
    }
}
