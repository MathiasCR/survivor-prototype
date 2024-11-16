using System;
using System.Collections.Generic;
using UnityEngine;

public class CrateGUI : MonoBehaviour
{
    [SerializeField] private Color _rareColor;
    [SerializeField] private Color _epicColor;
    [SerializeField] private Color _commonColor;
    [SerializeField] private Color _legendaryColor;
    [SerializeField] private GameObject _cratePanel;
    [SerializeField] private GameObject _crateCanvas;
    [SerializeField] private GameObject _itemStatsPanel;

    public event Action<int> OnSelectedItem;

    private List<GameObject> _itemPanels = new List<GameObject>();

    public void ShowCratePanel(List<WeaponMod> mods, GazFilterData gazFilter)
    {
        _crateCanvas.SetActive(true);
        SetupItemPanels(mods, gazFilter);
    }

    public void ShowWeaponCratePanel(List<WeaponData> weapons)
    {
        _crateCanvas.SetActive(true);
        SetupWeaponPanels(weapons);
    }

    public void HideCratePanel()
    {
        _crateCanvas.SetActive(false);
    }

    private void SetupItemPanels(List<WeaponMod> mods, GazFilterData gazFilter)
    {
        PlayerManager playerManager = GameManager.Instance.GetPlayerManager();
        WeaponManager equippedWeaponManager = playerManager.EquippedWeapon.GetComponent<WeaponManager>();
        List<CrateItemData> crateItemDatas = new List<CrateItemData>();
        int index = 0;

        foreach (WeaponMod mod in mods)
        {
            string replace = null;
            GameObject modStatPanel = Instantiate(_itemStatsPanel, _cratePanel.transform);
            _itemPanels.Add(modStatPanel);

            if (modStatPanel.TryGetComponent(out CrateItemGUI modItemGUI))
            {
                modItemGUI.OnItemSelected += OnItemSelected;

                string bonusStat = $"Bonus : {mod.BonusModifier.UpgradeType} {mod.BonusModifier.ModifierValue}";
                string malusStat = null;

                if (mod.ModData.ModMalusModifiers.Count > 0)
                {
                    malusStat = $"Malus : {mod.MalusModifier.UpgradeType} {mod.MalusModifier.ModifierValue}";
                }

                string replacingBonusStat = null;
                string replacingMalusStat = null;

                if (equippedWeaponManager != null)
                {
                    equippedWeaponManager.CheckModTypeOnWeapon(mod.ModData.ModType, out WeaponMod weaponMod);
                    if (weaponMod != null)
                    {
                        replace = $"Remplace {weaponMod.ModData.ModType}";

                        replacingBonusStat = $"Remplace Bonus : {weaponMod.BonusModifier.UpgradeType} {weaponMod.BonusModifier.ModifierValue}";

                        if (weaponMod.ModData.ModMalusModifiers.Count > 0)
                        {
                            replacingMalusStat = $"Remplace Malus : {weaponMod.MalusModifier.UpgradeType} {weaponMod.MalusModifier.ModifierValue}";
                        }
                    }
                }

                CrateItemData crateItemData = new CrateItemData
                {
                    Index = index,
                    Type = mod.ModData.ModType.ToString(),
                    Icon = mod.ModData.ModIcon,
                    Rarity = mod.ModData.ModRarity.ToString(),
                    BonusStat = bonusStat,
                    MalusStat = malusStat,
                    ReplaceType = replace,
                    ReplacingBonusStat = replacingBonusStat,
                    ReplacingMalusStat = replacingMalusStat
                };

                crateItemDatas.Add(crateItemData);
                modItemGUI.SetupItemData(crateItemData);
                index++;
            }
        }

        GameObject statPanel = Instantiate(_itemStatsPanel, _cratePanel.transform);
        _itemPanels.Add(statPanel);
        if (statPanel.TryGetComponent(out CrateItemGUI gazItemGUI))
        {
            gazItemGUI.OnItemSelected += OnItemSelected;

            CrateItemData crateItemData = new CrateItemData
            {
                Index = index,
                Type = gazFilter.GazFilterType.ToString(),
                Icon = gazFilter.GazFilterIcon,
                Rarity = gazFilter.GazFilterRarity.ToString(),
                BonusStat = $"Bonus de temps : {gazFilter.FilterTimerBonus / 60}min",
                MalusStat = null,
                ReplaceType = null,
                ReplacingBonusStat = null,
                ReplacingMalusStat = null,
            };

            crateItemDatas.Add(crateItemData);
            gazItemGUI.SetupItemData(crateItemData);
        }
    }

    private void SetupWeaponPanels(List<WeaponData> weapons)
    {
        int index = 0;
        foreach (WeaponData weaponData in weapons)
        {
            GameObject weaponPanel = Instantiate(_itemStatsPanel, _cratePanel.transform);
            _itemPanels.Add(weaponPanel);
            if (weaponPanel.TryGetComponent(out CrateItemGUI weaponGUI) && weaponData.Unlocked)
            {
                weaponGUI.OnItemSelected += OnItemSelected;

                WeaponCrateItemData weaponCrateItemData = new WeaponCrateItemData
                {
                    Index = index,
                    Icon = weaponData.WeaponIcon,
                    Type = weaponData.WeaponType.ToString(),
                    Rarity = weaponData.WeaponRarity.ToString(),
                    WeaponStats = new List<string>
                    {
                        { $"Range : {weaponData.Range}" },
                        { $"Damage : {weaponData.Damage}"},
                        { $"Number of Projectiles : {weaponData.Projectile}"},
                        { $"Magazine : {weaponData.MagSize}"},
                        { $"Fire Mode : {weaponData.WeaponFireMode}"},
                        { $"Fire Rate : {weaponData.FireRate}"},
                        { $"Reload Time : {weaponData.ReloadTime}"},
                        { $"Aiming Speed : {weaponData.AimingSpeed}"},
                        { $"Spread Accumulation : {weaponData.SpreadAccumulation}"},
                        { $"Maximum Spread : {weaponData.SpreadMax}"},
                        { $"Headshot Chance : {weaponData.HeadShotChance}"},
                        { $"Headshot Damage Multiplier : {weaponData.HeadShotDamageMultiplier}"},
                    }
                };

                weaponGUI.SetupWeaponData(weaponCrateItemData);
            }
            else
            {
                weaponGUI.SetupLockedWeaponData();
            }

            index++;
        }
    }

    public void OnItemSelected(int index)
    {
        foreach (GameObject itemPanel in _itemPanels)
        {
            if (itemPanel.TryGetComponent(out CrateItemGUI itemPanelGUI))
            {
                itemPanelGUI.OnItemSelected -= OnItemSelected;
            }

            Destroy(itemPanel);
        }
        _itemPanels.Clear();

        OnSelectedItem?.Invoke(index);
    }

    private Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonColor;
            case Rarity.Rare:
                return _rareColor;
            case Rarity.Epic:
                return _epicColor;
            case Rarity.Legendary:
                return _legendaryColor;
            default:
                return _commonColor;
        }
    }
}
