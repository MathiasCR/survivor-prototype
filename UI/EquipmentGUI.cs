using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGUI : MonoBehaviour
{
    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _previousBtn;
    [SerializeField] private Image _equipmentIcon;
    [SerializeField] private Image _notAllowedIcon;
    [SerializeField] private Equipment _emptyEquipment;
    [SerializeField] private TextMeshProUGUI _panelLabel;

    //public event Action<Equipment> OnEquipmentChanged;
    public EquipmentType EquipmentType;

    private int _index = 0;
    private PlayerGUI _playerGUI;
    private List<Equipment> _availableEquipmentsList;

    public void SetUpEquipmentPanel(EquipmentType type, PlayerGUI playerGUI)
    {
        _playerGUI = playerGUI;
        EquipmentType = type;
        _panelLabel.text = EquipmentType.ToString();
        _availableEquipmentsList = new List<Equipment>() { _emptyEquipment };

        //_availableEquipmentsList.AddRange(GameData.Instance.GetEquipmentsByType(type));
        _equipmentIcon.sprite = _availableEquipmentsList[0].EquipmentIcon;

        //Next equipment
        _nextBtn.onClick.AddListener(() =>
        {
            int nextIndex = (_index + 1) != _availableEquipmentsList.Count ? _index + 1 : 0;
            UpdateEquipmentPanel(nextIndex);
        });

        //Previous equipment
        _previousBtn.onClick.AddListener(() =>
        {
            int nextIndex = (_index - 1) < 0 ? _availableEquipmentsList.Count - 1 : _index - 1;
            UpdateEquipmentPanel(nextIndex);
        });
    }

    private void UpdateEquipmentPanel(int nextIndex)
    {
        Equipment previousEquipment = _availableEquipmentsList[_index];
        Equipment newEquipment = _availableEquipmentsList[nextIndex];

        _index = nextIndex;

        _equipmentIcon.sprite = newEquipment.EquipmentIcon;

        /*if (newEquipment.DoOthersAllowThis(_playerGUI.EquippedItems))
        {
            _playerGUI.SwitchPlayerEquipment(newEquipment, previousEquipment);
            OnEquipmentChanged?.Invoke(newEquipment);
        }
        else
        {
            _notAllowedIcon.gameObject.SetActive(true);
        }*/
    }

    public void CheckNewEquipmentRestrictions(Equipment equipment)
    {
        bool canStayEquipped = true;

        if (!equipment.CanHaveTop && EquipmentType == EquipmentType.Top)
        {
            canStayEquipped = false;
        }

        if (!equipment.CanHaveEyes && EquipmentType == EquipmentType.Eyes)
        {
            canStayEquipped = false;
        }

        if (!equipment.CanHaveEars && EquipmentType == EquipmentType.Ears)
        {
            canStayEquipped = false;
        }

        if (!equipment.CanHaveFace && EquipmentType == EquipmentType.Face)
        {
            canStayEquipped = false;
        }

        if (!equipment.CanHaveBody && EquipmentType == EquipmentType.Body)
        {
            canStayEquipped = false;
        }

        if (canStayEquipped)
        {
            if (_availableEquipmentsList[_index].EquipmentType == EquipmentType.None) return;
            _notAllowedIcon.gameObject.SetActive(false);
            //_playerGUI.EquipPlayer(_availableEquipmentsList[_index]);
        }
        else
        {
            if (_availableEquipmentsList[_index].EquipmentType == EquipmentType.None) return;
            _notAllowedIcon.gameObject.SetActive(true);
            //_playerGUI.UnEquipPlayer(_availableEquipmentsList[_index]);
        }
    }
}
