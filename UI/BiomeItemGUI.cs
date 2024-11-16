using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeItemGUI : MonoBehaviour
{
    [SerializeField] private Image _biomeIcon;
    [SerializeField] private Button _panelBtn;
    [SerializeField] private TextMeshProUGUI _biomeTypeText;

    public event Action<int> OnBiomeSelected;

    public void SetupBiomeData(Biome biome, int index)
    {
        if (biome.Unlocked)
        {
            _panelBtn.onClick.AddListener(() =>
            {
                OnBiomeSelected?.Invoke(index);
            });

            _biomeIcon.sprite = biome.BiomeIcon;
            _biomeTypeText.text = biome.BiomeType.ToString();
        }
        else
        {
            _panelBtn.interactable = false;
            _biomeIcon.sprite = null;
            _biomeTypeText.text = "???";
        }
    }
}
