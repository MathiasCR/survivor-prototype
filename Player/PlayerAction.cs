using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerGUI _playerGUI;
    [SerializeField] private PlayerManager _playerManager;

    public event Action OnEscapePressed;

    public int CurrentMag => _currentMag;
    public int MagSize => _equippedWeaponManager != null ? _equippedWeaponManager.MagSize : 0;

    private WeaponAction _equippedWeaponAction;
    private WeaponManager _equippedWeaponManager;

    private int _currentMag;

    private Coroutine _interactCoroutine;
    private Coroutine _reloadGUICoroutine;
    private Coroutine _playerAutoFireCoroutine;

    private void Awake()
    {
        _playerManager.OnEquippedWeaponChange += OnEquippedWeaponChange;
        _playerManager.OnRemoveEquippedWeapon += OnRemoveEquippedWeapon;
    }

    private void OnEquippedWeaponChange()
    {
        // Start switch animation with previous weapon
        if (_equippedWeaponAction != null) _equippedWeaponAction.SwitchWeapon();

        _equippedWeaponAction = _playerManager.EquippedWeapon.GetComponent<WeaponAction>();

        // Start switch animation with new weapon
        _equippedWeaponAction.SwitchWeapon();
        _equippedWeaponAction.OnSwitchEnded += OnSwitchEnded;

        _equippedWeaponManager = _playerManager.EquippedWeapon.GetComponent<WeaponManager>();
        _currentMag = _equippedWeaponManager.CurrentMag;
    }

    private void OnRemoveEquippedWeapon()
    {
        _equippedWeaponAction = null;
        _equippedWeaponManager = null;
        _currentMag = 0;
    }

    private void Update()
    {
        if (_playerManager.PlayerLife <= 0)
        {
            if (_playerAutoFireCoroutine != null) StopCoroutine(_playerAutoFireCoroutine);
        }
    }

    public void OnFire(InputValue value)
    {
        if (!_playerManager.CanFire) return;

        if (value.isPressed && !_playerManager.IsReloading && _currentMag > 0)
        {
            if (_equippedWeaponManager.WeaponFireMode == FireMode.Auto)
            {
                _playerAutoFireCoroutine = StartCoroutine(PlayerAutoFireCoroutine());
            }
            else
            {
                _playerManager.IsFiring = true;
                _currentMag--;
                _equippedWeaponAction.Fire(gameObject.layer, _playerManager.IsMoving, _playerManager.IsAiming);
                _equippedWeaponAction.OnFireEnded += OnFireEnded;
            }
        }
        else if (value.isPressed && _playerManager.IsReloading && _equippedWeaponManager.ReloadStoppable)
        {
            _equippedWeaponAction.StopReload();
        }
        else
        {
            if (_playerAutoFireCoroutine != null)
            {
                _playerManager.IsFiring = false;
                StopCoroutine(_playerAutoFireCoroutine);
            }
        }
    }

    private void OnFireEnded()
    {
        _playerManager.IsFiring = false;
        _equippedWeaponAction.OnFireEnded -= OnFireEnded;
    }

    public void OnAiming(InputValue value)
    {
        if (value.isPressed)
        {
            _playerManager.IsAiming = true;
        }
        else
        {
            _playerManager.IsAiming = false;
        }
    }

    private IEnumerator PlayerAutoFireCoroutine()
    {
        while (_currentMag > 0)
        {
            _equippedWeaponAction.Fire(gameObject.layer, _playerManager.IsMoving, _playerManager.IsAiming);
            _currentMag--;
            yield return new WaitForSeconds(_equippedWeaponManager.FireRate);
        }
    }

    public void OnReload()
    {
        if (!_playerManager.CanDoAction
            || _equippedWeaponManager.MagSize == _currentMag
            ) return;

        _playerManager.IsReloading = true;
        if (_equippedWeaponManager.ReloadStoppable)
        {
            _equippedWeaponAction.StartContinuousReload();
            _reloadGUICoroutine = StartCoroutine(ReloadGUI());
            _equippedWeaponAction.OnReloadEnded += OnReloadEnded;
            _equippedWeaponAction.OnAmmoLoaded += OnAmmoLoaded;
        }
        else
        {
            _equippedWeaponAction.Reload();
            _reloadGUICoroutine = StartCoroutine(ReloadGUI());
            _equippedWeaponAction.OnReloadEnded += OnReloadEnded;
        }
    }

    private IEnumerator ReloadGUI()
    {
        float remainingTime = 0f;

        while (remainingTime < _equippedWeaponManager.ReloadTime)
        {
            remainingTime += 0.1f;
            _playerGUI.UpdateReloadTimer(remainingTime / _equippedWeaponManager.ReloadTime);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnAmmoLoaded()
    {
        _currentMag = _equippedWeaponManager.CurrentMag;
        if (_currentMag < _equippedWeaponManager.MagSize)
        {
            _reloadGUICoroutine = StartCoroutine(ReloadGUI());
        }
    }

    private void OnReloadEnded()
    {
        StopCoroutine(_reloadGUICoroutine);
        _playerGUI.UpdateReloadTimer(0f);
        _equippedWeaponAction.OnReloadEnded -= OnReloadEnded;
        _equippedWeaponAction.OnAmmoLoaded -= OnAmmoLoaded;
        _currentMag = _equippedWeaponManager.CurrentMag;
        _playerManager.IsReloading = false;
    }

    private void OnSwitchEnded()
    {
        _equippedWeaponAction.OnSwitchEnded -= OnSwitchEnded;
        _playerManager.IsSwitchingWeapon = false;
    }

    public void OnInteraction(InputValue value)
    {
        if (value.isPressed && _playerManager.CanDoAction && _playerManager.Interactables.Count > 0)
        {
            if (_playerManager.Interactables[0].TryGetComponent(out Interactable interactable))
            {
                if (interactable.HoldToUse)
                {
                    _playerManager.IsInteracting = true;
                    _interactCoroutine = StartCoroutine(HoldToInteract(interactable));
                }
                else
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            if (_interactCoroutine != null)
            {
                _playerManager.IsInteracting = false;
                StopCoroutine(_interactCoroutine);
                _playerGUI.UpdateInteractionTimer(0f);
            }
        }
    }

    private IEnumerator HoldToInteract(Interactable interactable)
    {
        float remainingTime = 0f;

        while (remainingTime < interactable.HoldTimer)
        {
            remainingTime += 0.1f;
            _playerGUI.UpdateInteractionTimer(remainingTime / interactable.HoldTimer);
            yield return new WaitForSeconds(.1f);
        }

        _playerManager.IsInteracting = false;
        interactable.Interact();
        _playerGUI.UpdateInteractionTimer(0f);
    }

    public void OnEscape()
    {
        OnEscapePressed?.Invoke();
    }
}
