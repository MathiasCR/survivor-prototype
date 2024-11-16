using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopCrateBehaviour : Interactable
{
    [SerializeField] private Rarity _rarity;
    [SerializeField] private CrateGUI _crateGUI;

    public event Action<WeaponMod, WeaponMod> OnPlayerGetMod;
    public event Action<GazFilterData> OnPlayerGetFilterGaz;

    private WeaponMod _previousMod;
    private WeaponMod _selectedMod;
    private GazFilterData _gazFilter;
    private PlayerManager _playerManager;
    private CrateState _currentCrateState;
    private List<WeaponMod> _lootedMods = new List<WeaponMod>();

    private void Start()
    {
        _currentCrateState = CrateState.Inactive;
        _playerManager = GameManager.Instance.GetPlayerManager();

        _crateGUI.OnSelectedItem += OnItemSelected;
    }

    private void ChangeCrateState(CrateState newValue)
    {
        _currentCrateState = newValue;

        switch (newValue)
        {
            case CrateState.Inactive:
                animator.SetBool("Open", false);
                break;
            case CrateState.Activated:
                animator.SetBool("Open", true);
                GenerateRandomItems();
                break;
        }
    }

    private void GenerateRandomItems()
    {
        int defaultNbr = 2;
        int luck = new System.Random().Next(0, 100);

        if (luck < _playerManager.Luck)
        {
            defaultNbr++;
        }

        for (int i = 0; i < defaultNbr; i++)
        {
            List<WeaponModData> mods = GameData.Instance.GetWeaponModDataByRarityForPlayerWeapons(_rarity);
            WeaponModData mod = mods[UnityEngine.Random.Range(0, mods.Count)];

            WeaponMod lootedMod = new WeaponMod(mod);
            lootedMod.BonusModifier = mod.ModBonusModifiers[UnityEngine.Random.Range(0, mod.ModBonusModifiers.Count)];
            if (mod.ModMalusModifiers.Count > 0)
            {
                lootedMod.MalusModifier = mod.ModMalusModifiers[UnityEngine.Random.Range(0, mod.ModMalusModifiers.Count)];
            }

            _lootedMods.Add(lootedMod);
        }

        // Ajouter un filtre de rareté random
        List<GazFilterData> gazFilters = GameData.Instance.GetGazFilterDataByRarity(_rarity);
        _gazFilter = gazFilters[UnityEngine.Random.Range(0, gazFilters.Count)];

        // Afficher le Crate panel en passant les infos des mods + du filtre
        GameManager.Instance.PauseGame();
        _playerManager.IsInMenu = true;
        _crateGUI.ShowCratePanel(_lootedMods, _gazFilter);
    }

    public void OnItemSelected(int index)
    {
        Debug.Log("HEHEHEHE.... Thank you");

        // Récupérer l'item selectionné
        if (index < _lootedMods.Count)
        {
            _selectedMod = _lootedMods[index];
            OnPlayerGetMod?.Invoke(_previousMod, _selectedMod);
            _selectedMod = null;
        }
        else
        {
            OnPlayerGetFilterGaz?.Invoke(_gazFilter);
        }

        _previousMod = null;
        _lootedMods.Clear();
        _crateGUI.HideCratePanel();
        _playerManager.IsInMenu = false;
        GameManager.Instance.ResumeGame();
        ChangeCrateState(CrateState.Inactive);
    }

    public override void Interact()
    {
        int tradeItem = GameManager.Instance.GetTradeItemByRarity(_rarity);

        if (_currentCrateState == CrateState.Inactive && tradeItem > 0)
        {
            GameManager.Instance.RemoveTradeItemByRarity(_rarity);
            ChangeCrateState(CrateState.Activated);
        }
        else
        {
            Debug.Log("NOT ENOUGH CASH.......... stranger");
        }
    }
}
