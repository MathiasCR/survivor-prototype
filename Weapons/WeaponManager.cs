using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private WeaponAction _weaponAction;
    [SerializeField] private List<GameObject> _weaponMods;
    [SerializeField] private WeaponAnimator _weaponAnimator;
    [SerializeField] private SkinnedMeshRenderer _weaponRenderer;

    [Header("Base Weapon Settings (Read Only)")]
    [SerializeField] public int Damage;
    [SerializeField] public int Bullet;
    [SerializeField] public int MagSize;
    [SerializeField] public float StunTimer;
    [SerializeField] public bool ReloadStoppable;
    [SerializeField] public FireMode WeaponFireMode;

    [Header("Weapon HeadShot Settings (Read Only)")]
    [SerializeField] public int HeadShotChance;
    [SerializeField] public int HeadShotDamageMultiplier;

    [Header("Weapon Type & Rarity (Read Only)")]
    [SerializeField] public Rarity WeaponRarity;
    [SerializeField] public WeaponType WeaponType;

    [Header("Weapon Speed Settings (Read Only)")]
    [SerializeField] public float FireRate;
    [SerializeField] public float ReloadTime;
    [SerializeField] public float AimingSpeed;
    [SerializeField] public float ReloadTransitionTime;

    [Header("Weapon Spread & Range (Read Only)")]
    [SerializeField] public float Range;
    [SerializeField] public int SpreadMax;
    [SerializeField] public float SpreadAccumulation;

    [Header("Weapon Prefabs (Read Only)")]
    [SerializeField] public Sprite WeaponIcon;
    [SerializeField] public AudioClip FireAudio;
    [SerializeField] public GameObject WeaponPrefab;
    [SerializeField] public GameObject BulletPrefab;

    public event Action OnSpreadMaxChange;
    public event Action OnSpreadAccumulationChange;

    public bool IsOwnedByPlayer = false;

    public int CurrentMag
    {
        get => _currentMag;
    }

    public bool IsRendered
    {
        get => _isRendered;
    }

    public float MaxReloadTime
    {
        get => ReloadTime * (MagSize - _currentMag);
    }

    private int _currentMag;
    private bool _isRendered;
    private Dictionary<GameObject, WeaponMod> _equippedWeaponMods = new Dictionary<GameObject, WeaponMod>();

    private void Awake()
    {
        gameObject.name = Utils.RemoveCloneFromName(gameObject.name);

        InitializeWeaponData();

        _currentMag = MagSize;
    }

    private void InitializeWeaponData()
    {
        Range = _weaponData.Range;
        Damage = _weaponData.Damage;
        Bullet = _weaponData.Projectile;
        MagSize = _weaponData.MagSize;
        FireRate = _weaponData.FireRate;
        StunTimer = _weaponData.StunTimer;
        FireAudio = _weaponData.FireAudio;
        SpreadMax = _weaponData.SpreadMax;
        WeaponIcon = _weaponData.WeaponIcon;
        ReloadTime = _weaponData.ReloadTime;
        WeaponType = _weaponData.WeaponType;
        AimingSpeed = _weaponData.AimingSpeed;
        WeaponRarity = _weaponData.WeaponRarity;
        WeaponFireMode = _weaponData.WeaponFireMode;
        HeadShotChance = _weaponData.HeadShotChance;
        ReloadStoppable = _weaponData.ReloadStoppable;
        SpreadAccumulation = _weaponData.SpreadAccumulation;
        HeadShotDamageMultiplier = _weaponData.HeadShotDamageMultiplier;

        OnSpreadMaxChange?.Invoke();
        OnSpreadAccumulationChange?.Invoke();

        _weaponAnimator.SetAnimationModifiers(FireRate, ReloadTime);
    }

    private void UpdateWeaponData(bool addMod, WeaponMod weaponMod)
    {
        ApplyModifier(addMod, weaponMod.BonusModifier);

        if (weaponMod.ModData.ModMalusModifiers.Count > 0)
        {
            ApplyModifier(addMod, weaponMod.MalusModifier);
        }

        //Bullet += add ? +modData.Bullet : -modData.Bullet;

        if (weaponMod.ModData.FireAudio != null)
        {
            FireAudio = addMod ? weaponMod.ModData.FireAudio : _weaponData.FireAudio;
        }

        _weaponAnimator.SetAnimationModifiers(FireRate, ReloadTime);
    }

    private void ApplyModifier(bool add, Modifier modifier)
    {
        switch (modifier.UpgradeType)
        {
            case UpgradeType.Range:
                Range += add ? +modifier.ModifierValue : -modifier.ModifierValue;
                if (Range < 0.1f) Range = 0.1f;
                break;
            case UpgradeType.Damage:
                Damage += add ? +(int)Math.Ceiling(modifier.ModifierValue) : -(int)Math.Ceiling(modifier.ModifierValue);
                if (Damage < 0) Damage = 0;
                break;
            case UpgradeType.FireRate:
                FireRate += add ? +modifier.ModifierValue : -modifier.ModifierValue;
                if (FireRate < 0.1f) FireRate = 0.1f;
                break;
            case UpgradeType.FireMode:
                WeaponFireMode = add ? ((FireMode)(int)Math.Ceiling(modifier.ModifierValue)) : _weaponData.WeaponFireMode;
                break;
            case UpgradeType.Magazine:
                MagSize = add ? (int)Math.Ceiling(modifier.ModifierValue) : _weaponData.MagSize;
                break;
            case UpgradeType.Reload:
                ReloadTime += add ? +modifier.ModifierValue : -modifier.ModifierValue;
                if (ReloadTime < 0.1f) ReloadTime = 0.1f;
                break;
            case UpgradeType.AimingSpeed:
                AimingSpeed += add ? +modifier.ModifierValue : -modifier.ModifierValue;
                if (AimingSpeed < 0.1f) AimingSpeed = 0.1f;
                break;
            case UpgradeType.SpreadMax:
                SpreadMax += add ? +(int)Math.Ceiling(modifier.ModifierValue) : -(int)Math.Ceiling(modifier.ModifierValue);
                if (SpreadMax < 0) SpreadMax = 0;
                OnSpreadMaxChange?.Invoke();
                break;
            case UpgradeType.SpreadAcc:
                SpreadAccumulation += add ? +modifier.ModifierValue : -modifier.ModifierValue;
                if (SpreadAccumulation < 0.1f) SpreadAccumulation = 0.1f;
                OnSpreadAccumulationChange?.Invoke();
                break;
            case UpgradeType.Special:
                break;
            case UpgradeType.HeadshotChance:
                HeadShotChance += add ? +(int)Math.Ceiling(modifier.ModifierValue) : -(int)Math.Ceiling(modifier.ModifierValue);
                if (HeadShotChance < 0) HeadShotChance = 0;
                break;
            case UpgradeType.HeadshotDamage:
                HeadShotDamageMultiplier += add ? +(int)Math.Ceiling(modifier.ModifierValue) : -(int)Math.Ceiling(modifier.ModifierValue);
                if (HeadShotDamageMultiplier < 0) HeadShotDamageMultiplier = 0;
                break;
        }
    }

    public void UpdateMag(int nb)
    {
        _currentMag += nb;
    }

    public void ReinitializeMag()
    {
        _currentMag = MagSize;
    }

    /// <summary>
    /// Remplace le previousModPrefab si existant par le newModPrefab
    /// Sinon ajoute simplement le newModPrefab
    /// </summary>
    /// <param name="previousModPrefab">ancien mod à retirer de l'arme (peut être null)</param>
    /// <param name="newModPrefab">nouveau mod à ajouter à l'arme</param>
    public void UpdateWeaponStatsAndVisual(WeaponMod previousMod, WeaponMod newMod)
    {
        if (previousMod != null)
        {
            GameObject previousWeaponMod = _equippedWeaponMods.FirstOrDefault(x => x.Value == previousMod).Key;

            ChangeModVisibility(previousWeaponMod, false);
            _equippedWeaponMods.Remove(previousWeaponMod);
            UpdateWeaponData(false, previousMod);
        }

        GameObject newWeaponMod = _weaponMods.Find((mod) => mod.name == newMod.ModData.name);

        ChangeModVisibility(newWeaponMod, true);
        _equippedWeaponMods.Add(newWeaponMod, newMod);
        UpdateWeaponData(true, newMod);
    }

    public void ChangeWeaponVisibility(bool isVisible)
    {
        ChangeAllModsVisibility(isVisible);
        _weaponRenderer.enabled = isVisible;
        _isRendered = isVisible;
    }

    public void CheckModTypeOnWeapon(ModType modType, out WeaponMod weaponMod)
    {
        weaponMod = null;

        foreach (WeaponMod mod in _equippedWeaponMods.Values)
        {
            if (mod.ModData.ModType == modType)
            {
                weaponMod = mod;
            }
        }
    }

    public void ChangeModVisibility(GameObject mod, bool isVisible)
    {
        SkinnedMeshRenderer meshRenderer = mod.GetComponent<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = isVisible;
        }
    }

    public void ChangeAllModsVisibility(bool isVisible)
    {
        foreach (GameObject mod in _equippedWeaponMods.Keys)
        {
            ChangeModVisibility(mod, isVisible);
        }
    }

    public void RemoveWeapon()
    {
        Destroy(gameObject);
    }
}
