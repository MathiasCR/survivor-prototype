using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/WeaponScriptableObject", order = 3)]
public class WeaponData : ScriptableObject
{
    [Header("Base Weapon Settings")]
    [SerializeField] public int Damage;
    [SerializeField] public int MagSize;
    [SerializeField] public bool Unlocked;
    [SerializeField] public int Projectile;
    [SerializeField] public float StunTimer;
    [SerializeField] public bool ReloadStoppable;
    [SerializeField] public FireMode WeaponFireMode;

    [Header("Weapon HeadShot Settings")]
    [SerializeField] public int HeadShotChance;
    [SerializeField] public int HeadShotDamageMultiplier;

    [Header("Weapon Type & Rarity")]
    [SerializeField] public Rarity WeaponRarity;
    [SerializeField] public WeaponType WeaponType;

    [Header("Weapon Speed Settings")]
    [SerializeField] public float FireRate;
    [SerializeField] public float ReloadTime;
    [SerializeField] public float AimingSpeed;

    [Header("Weapon Spread & Range")]
    [SerializeField] public float Range;
    [SerializeField] public int SpreadMax;
    [SerializeField] public float SpreadAccumulation;

    [Header("Weapon Prefabs")]
    [SerializeField] public Sprite WeaponIcon;
    [SerializeField] public AudioClip FireAudio;
    [SerializeField] public GameObject WeaponPrefab;
    [SerializeField] public GameObject BulletPrefab;
}

public enum FireMode
{
    Single,
    Burst,
    Auto
}

public enum WeaponType
{
    Pistol,
    Shotgun
}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

