using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private Image _reloadTimer;
    [SerializeField] private Transform _playerHud;
    [SerializeField] private Image _interactionTimer;
    [SerializeField] private TextMeshProUGUI _magInfo;
    [SerializeField] private GameObject _interactionBtn;
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private TextMeshProUGUI _interactionInfo;

    private void Update()
    {
        if (_playerAction.MagSize > 0)
        {
            _magInfo.text = $"{_playerAction.CurrentMag}/{_playerAction.MagSize}";
        }

        if (_playerManager.Interactables.Count > 0 && _playerManager.Interactables[0] != null
            && _playerManager.Interactables[0].TryGetComponent(out Interactable interactable))
        {
            ChangeKeyInteractionVisibility(true);
            _interactionInfo.text = interactable.InteractionText;
        }
        else
        {
            ChangeKeyInteractionVisibility(false);
            _interactionInfo.text = "";
        }

        Vector3 rotation = Camera.main.transform.position;
        _playerHud.LookAt(rotation);
    }

    public void ChangeLifeSliderVisibility(bool visibility)
    {
        _lifeSlider.gameObject.SetActive(visibility);
    }

    public void SetPlayerLife(int playerLife)
    {
        _lifeSlider.maxValue = playerLife;
        _lifeSlider.value = playerLife;
    }

    public void UpdatePlayerLife(int playerLife)
    {
        _lifeSlider.value = playerLife;
    }

    public void UpdateReloadTimer(float value)
    {
        _reloadTimer.fillAmount = value;
    }

    public void ChangeKeyInteractionVisibility(bool visible)
    {
        _interactionBtn.SetActive(visible);
    }

    public void UpdateInteractionTimer(float value)
    {
        _interactionTimer.fillAmount = value;
    }

    public void EmptyMagInfo()
    {
        _magInfo.text = "";
    }
}
