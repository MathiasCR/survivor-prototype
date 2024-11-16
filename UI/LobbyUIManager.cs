using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Toggle _toggleColor;
    [SerializeField] private GameObject _lobbyUI;
    [SerializeField] private GameObject _colorPanel;
    [SerializeField] private GameObject _equipmentsPanel;
    [SerializeField] private GameObject _equipmentPanelPrefab;

    private PlayerGUI _playerGUI;
    private List<EquipmentGUI> _equipmentGUIs = new List<EquipmentGUI>();

    private void Awake()
    {
        _playerGUI = GameManager.Instance.GetPlayerGUI();

        /*foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
        {
            if (type == EquipmentType.None) break;
            GameObject equipmentPanel = Instantiate(_equipmentPanelPrefab, _equipmentsPanel.transform);
            _equipmentGUIs.Add(equipmentPanel.GetComponent<EquipmentGUI>());
            _equipmentGUIs.Last().SetUpEquipmentPanel(type, _playerGUI);
            _equipmentGUIs.Last().OnEquipmentChanged += CheckCompatibilityWithOtherEquipments;
        }*/

        foreach (Color color in Utils.GetColors())
        {
            Toggle toggle = Instantiate(_toggleColor, _colorPanel.transform);

            if (_colorPanel.TryGetComponent(out ToggleGroup toggleGroup))
            {
                ColorBlock cb = toggle.colors;
                cb.normalColor = color;
                cb.selectedColor = color;
                cb.highlightedColor = color;
                cb.pressedColor = color;

                toggle.colors = cb;
                toggle.group = toggleGroup;

                toggle.onValueChanged.AddListener((bool isActive) =>
                {
                });
            }
        }
    }
}
