using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private PlayerGUI _playerGUI;
    [SerializeField] private BoxCollider _playerCollider;

    [Header("Life Player Settings (Read Only)")]
    [SerializeField] public int MaxLife;
    [SerializeField] public int Luck;
    [SerializeField] public float Sway;
    [SerializeField] public float MagnetRadius;
    [SerializeField] public float DefaultFilterTimer;

    [Header("Speed Player Settings (Read Only)")]
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float ReloadSpeed;
    [SerializeField] public float AimingMoveSpeed;
    [SerializeField] public float SwitchWeaponSpeed;

    [Header("Animation Player Settings (Read Only)")]
    [SerializeField] public float MoveAnimationSpeed;

    public event Action OnCharacterChange;
    public event Action OnEquippedWeaponChange;
    public event Action OnRemoveEquippedWeapon;

    public GameObject CharacterPrefab
    {
        get => _characterPrefab;
    }

    public GameObject EquippedWeapon
    {
        get => _equippedWeapon;
    }

    public bool IsMoving = false;
    public bool IsInMenu = false;
    public bool IsAiming = false;
    public bool IsFiring = false;
    public bool IsReloading = false;
    public bool IsInteracting = false;
    public bool IsSwitchingWeapon = false;

    public bool CanMove => !IsInMenu;
    public bool CanFire => !IsSwitchingWeapon && !IsInMenu && !IsFiring && EquippedWeapon != null;
    public bool CanDoAction => !IsReloading && !IsSwitchingWeapon && !IsInMenu && !IsFiring && !IsInteracting;

    public int PlayerLife { get => _playerLife; }
    public List<Collider> Interactables = new List<Collider>();

    private int _playerLife;
    private PlayerData _playerData;
    private SkinnedMeshRenderer _hands;
    private GameObject _equippedWeapon;
    private GameObject _characterPrefab;

    private void Awake()
    {
        InvokeRepeating(nameof(GetInteractableColliders), 0.1f, 0.2f);
    }

    public void SetPlayerCharacter(GameObject characterPrefab)
    {
        if (characterPrefab != null)
        {
            Destroy(_characterPrefab);
        }

        RemoveEquippedWeapon();

        _characterPrefab = Instantiate(characterPrefab, transform);
        _characterPrefab.name = Utils.RemoveCloneFromName(characterPrefab.name);
        _playerData = GameData.Instance.GetCharacterData(characterPrefab.name);

        SkinnedMeshRenderer[] meshRenderers = _characterPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.name == "Hands")
            {
                _hands = meshRenderer;
                break;
            }
        }

        if (_hands == null) Debug.LogError("Hands not found in character prefab");

        Luck = _playerData.Luck;
        Sway = _playerData.Sway;
        MaxLife = _playerData.MaxLife;
        MoveSpeed = _playerData.MoveSpeed;
        ReloadSpeed = _playerData.ReloadSpeed;
        MagnetRadius = _playerData.MagnetRadius;
        AimingMoveSpeed = _playerData.AimingMoveSpeed;
        SwitchWeaponSpeed = _playerData.SwitchWeaponSpeed;
        DefaultFilterTimer = _playerData.DefaultFilterTimer;
        MoveAnimationSpeed = _playerData.MoveAnimationSpeed;

        ResetPlayerLife();

        OnCharacterChange?.Invoke();
    }

    private void GetInteractableColliders()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 1.2f, 0f), 2f, GameLayers.Instance.InteractableLayer);
        Interactables.Clear();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Interactable interactable))
            {
                if (interactable.IsInteractable)
                {
                    Interactables.Add(collider);
                }
            }
        }
    }

    public void PlayerHit(int damage)
    {
        _playerLife -= damage;
        _playerGUI.UpdatePlayerLife(_playerLife);

        if (_playerLife <= 0)
        {
            _playerLife = 0;
            GameManager.Instance.OnPlayerDead();
        }
    }

    public void PlayerHeal(int heal)
    {
        _playerLife += heal;
        _playerGUI.UpdatePlayerLife(_playerLife);
    }

    public void ResetPlayerLife()
    {
        _playerLife = MaxLife;
        _playerGUI.SetPlayerLife(_playerLife);
    }

    public void ChangeHealthVisibility(bool visibility)
    {
        _playerGUI.ChangeLifeSliderVisibility(visibility);
    }

    public void RemoveEquippedWeapon()
    {
        if (_equippedWeapon != null && _equippedWeapon.TryGetComponent(out WeaponManager weaponManager))
        {
            weaponManager.RemoveWeapon();
        }
        _equippedWeapon = null;
        _playerGUI.EmptyMagInfo();
        OnRemoveEquippedWeapon?.Invoke();

        if (_hands != null) _hands.enabled = true;
    }

    public void EquipNewWeapon(GameObject weapon)
    {
        RemoveEquippedWeapon();

        GameObject weaponGO = Instantiate(weapon, gameObject.transform);
        if (weapon.TryGetComponent(out WeaponManager newWeaponManager))
        {
            newWeaponManager.ChangeWeaponVisibility(false);
            newWeaponManager.IsOwnedByPlayer = true;
        }

        if (_hands.enabled) _hands.enabled = false;
        _equippedWeapon = weaponGO;
        OnEquippedWeaponChange?.Invoke();
    }

    public void AddModToEquippedWeapon(WeaponMod previousMod, WeaponMod mod)
    {
        if (_equippedWeapon != null && _equippedWeapon.TryGetComponent(out WeaponManager weaponManager))
        {
            weaponManager.UpdateWeaponStatsAndVisual(previousMod, mod);
        }
    }
}
