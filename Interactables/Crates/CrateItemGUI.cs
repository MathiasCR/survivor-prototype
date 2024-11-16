using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrateItemGUI : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Button _panelBtn;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemRarityText;

    public event Action<int> OnItemSelected;

    public void SetupItemData(CrateItemData crateItemData)
    {
        _panelBtn.onClick.AddListener(() =>
        {
            OnItemSelected?.Invoke(crateItemData.Index);
        });

        _itemIcon.sprite = crateItemData.Icon;
        _itemTypeText.text = crateItemData.Type;
        _itemRarityText.text = crateItemData.Rarity;

        if (crateItemData.BonusStat != null)
        {
            GameObject statGo = Instantiate(new GameObject(), _statsPanel.transform);
            TextMeshProUGUI statText = statGo.AddComponent<TextMeshProUGUI>();
            statText.text = crateItemData.BonusStat;
            statText.color = Color.green;
        }

        if (crateItemData.MalusStat != null)
        {
            GameObject statGo = Instantiate(new GameObject(), _statsPanel.transform);
            TextMeshProUGUI statText = statGo.AddComponent<TextMeshProUGUI>();
            statText.text = crateItemData.MalusStat;
            statText.color = Color.red;
        }

        if (crateItemData.ReplaceType != null)
        {
            GameObject replaceGo = Instantiate(new GameObject(), _statsPanel.transform);
            TextMeshProUGUI replaceText = replaceGo.AddComponent<TextMeshProUGUI>();
            replaceText.text = crateItemData.ReplaceType;

            if (crateItemData.ReplacingBonusStat != null)
            {
                GameObject statGo = Instantiate(new GameObject(), _statsPanel.transform);
                TextMeshProUGUI statText = statGo.AddComponent<TextMeshProUGUI>();
                statText.text = crateItemData.ReplacingBonusStat;
                statText.color = Color.green;
            }

            if (crateItemData.ReplacingMalusStat != null)
            {
                GameObject statGo = Instantiate(new GameObject(), _statsPanel.transform);
                TextMeshProUGUI statText = statGo.AddComponent<TextMeshProUGUI>();
                statText.text = crateItemData.ReplacingMalusStat;
                statText.color = Color.red;
            }
        }
    }

    public void SetupWeaponData(WeaponCrateItemData weaponCrateItemData)
    {
        _panelBtn.onClick.AddListener(() =>
        {
            OnItemSelected?.Invoke(weaponCrateItemData.Index);
        });

        _itemIcon.sprite = weaponCrateItemData.Icon;
        _itemTypeText.text = weaponCrateItemData.Type;
        _itemRarityText.text = weaponCrateItemData.Rarity;

        foreach (string stat in weaponCrateItemData.WeaponStats)
        {
            GameObject statGo = Instantiate(new GameObject(), _statsPanel.transform);
            TextMeshProUGUI statText = statGo.AddComponent<TextMeshProUGUI>();
            statText.text = stat;
        }
    }

    public void SetupLockedWeaponData()
    {
        _itemIcon.sprite = null;
        _itemTypeText.text = "???";
        _itemRarityText.text = "???";
        _panelBtn.interactable = false;
    }
}

public struct CrateItemData
{
    public int Index;
    public Sprite Icon;
    public string Type;
    public string Rarity;
    public string BonusStat;
    public string MalusStat;
    public string ReplaceType;
    public string ReplacingBonusStat;
    public string ReplacingMalusStat;
}

public struct WeaponCrateItemData
{
    public int Index;
    public Sprite Icon;
    public string Type;
    public string Rarity;
    public List<string> WeaponStats;
}
