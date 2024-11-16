using System.Collections.Generic;
using UnityEngine;

public class FixedCrateBehaviour : Interactable
{
    [SerializeField] private CrateGUI _crateGUI;
    [SerializeField] private List<WeaponData> _weaponsData;

    private PlayerManager _playerManager;

    private void Start()
    {
        if (isInteractable)
        {
            animator.SetBool("Open", true);
            _playerManager = GameManager.Instance.GetPlayerManager();
        }
    }

    public override void Interact()
    {
        _playerManager.IsInMenu = true;
        _crateGUI.ShowWeaponCratePanel(_weaponsData);
        _crateGUI.OnSelectedItem += OnItemSelected;
    }

    private void OnItemSelected(int index)
    {
        if (_weaponsData.Count > index)
        {
            _playerManager.EquipNewWeapon(_weaponsData[index].WeaponPrefab);
            _playerManager.IsInMenu = false;
            _crateGUI.HideCratePanel();
        }
    }
}
