using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mod", menuName = "ScriptableObjects/ModScriptableObject", order = 4)]
public class WeaponModData : ScriptableObject
{
    [Header("Mod Type & Rarity")]
    [SerializeField] public ModType ModType;
    [SerializeField] public Rarity ModRarity;
    [SerializeField] public List<WeaponType> CompatibleWeaponTypes;

    [Header("Modifiers Settings")]
    [SerializeField] public List<Modifier> ModBonusModifiers;
    [SerializeField] public List<Modifier> ModMalusModifiers;

    [Header("Mod Prefabs")]
    [SerializeField] public Sprite ModIcon;
    [SerializeField] public AudioClip FireAudio;
}

[Serializable]
public struct Modifier
{
    public Sprite UpgradeIcon;
    public float ModifierValue;
    public UpgradeType UpgradeType;
}

public enum ModType
{
    Grip,
    Sight,
    Laser,
    Muzzle,
    Special,
    Trigger,
    Magazine,
    ButtStock,
    FlashLight,
}

public enum UpgradeType
{
    Range,
    Reload,
    Damage,
    Special,
    FireRate,
    Magazine,
    FireMode,
    SpreadAcc,
    SpreadMax,
    AimingSpeed,
    HeadshotDamage,
    HeadshotChance,
}
