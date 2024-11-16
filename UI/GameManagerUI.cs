using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private Image _sceneFader;
    [SerializeField] private GameObject _lobbyBtn;
    [SerializeField] private GameObject _biomePanel;
    [SerializeField] private GameObject _nextLevelPanel;
    [SerializeField] private GameObject _gameInfosPanel;
    [SerializeField] private GameObject _menuOptionsPanel;
    [SerializeField] private TextMeshProUGUI _elapsedTime;
    [SerializeField] private TextMeshProUGUI _extractions;
    [SerializeField] private TextMeshProUGUI _remainingTime;
    [SerializeField] private GridLayoutGroup _lootLayoutGroup;
    [SerializeField] private GridLayoutGroup _killsLayoutGroup;
    [SerializeField] private TextMeshProUGUI _commonTradeItems;
    [SerializeField] private TextMeshProUGUI _rareTradeItems;
    [SerializeField] private TextMeshProUGUI _epicTradeItems;
    [SerializeField] private List<GameObject> _gameInfoObjects;
    [SerializeField] private TextMeshProUGUI _legendaryTradeItems;

    public event Action<int> OnNextLevelSelected;

    private List<GameObject> _biomePanels = new List<GameObject>();

    public void ChangeRemainingTimeVisibilty(bool visible)
    {
        _remainingTime.gameObject.SetActive(visible);
    }

    public void UpdateRemainingTime(float remainingTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
        _remainingTime.text = timeSpan.ToString(@"mm\:ss");
    }

    public void ShowNextLevelPanelVisibility()
    {
        _nextLevelPanel.SetActive(true);
        SetupBiomePanel();
    }

    private void HideNextLevelPanelVisibility()
    {
        _nextLevelPanel.SetActive(false);
    }

    private void SetupBiomePanel()
    {
        List<Biome> biomes = GameData.Instance.Biomes;
        int index = 0;
        foreach (Biome biome in biomes)
        {
            GameObject biomePanel = Instantiate(_biomePanel, _nextLevelPanel.transform);
            _biomePanels.Add(biomePanel);

            if (biomePanel.TryGetComponent(out BiomeItemGUI biomeItemGUI))
            {
                biomeItemGUI.OnBiomeSelected += OnBiomeSelected;

                biomeItemGUI.SetupBiomeData(biome, index);
            }

            index++;
        }
    }

    private void OnBiomeSelected(int index)
    {
        HideNextLevelPanelVisibility();

        foreach (GameObject biomePanel in _biomePanels)
        {
            if (biomePanel.TryGetComponent(out BiomeItemGUI biomeItemGUI))
            {
                biomeItemGUI.OnBiomeSelected -= OnBiomeSelected;
            }

            Destroy(biomePanel);
        }
        _biomePanels.Clear();

        OnNextLevelSelected?.Invoke(index);
    }

    public void ChangeOptionsMenuPanelVisibility(bool visible, bool showLobbyBtn)
    {
        _menuOptionsPanel.SetActive(visible);

        if (visible && showLobbyBtn)
        {
            _lobbyBtn.SetActive(true);
        }
        else if (visible && !showLobbyBtn)
        {
            _lobbyBtn.SetActive(false);
        }
    }

    public void ChangeGameInfosPanelVisibility(bool visible)
    {
        _gameInfosPanel.SetActive(visible);

        if (!visible)
        {
            foreach (GameObject gameInfoObject in _gameInfoObjects)
            {
                Destroy(gameInfoObject);
            }
            _gameInfoObjects.Clear();
        }
    }

    public void SetGameInformationsData(GameInfo gameInfo)
    {
        foreach (EnemyType enemyType in gameInfo.killsByEnemyType.Keys)
        {
            InstantiateTextWithinGridLayoutGroup(_killsLayoutGroup, enemyType.ToString(), gameInfo.killsByEnemyType[enemyType].ToString());
        }

        foreach (LootType lootType in gameInfo.lootsByLootType.Keys)
        {
            InstantiateTextWithinGridLayoutGroup(_lootLayoutGroup, lootType.ToString(), gameInfo.lootsByLootType[lootType].ToString());
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(gameInfo.TimeElapsed);
        _elapsedTime.text = timeSpan.ToString(@"mm\:ss");

        _extractions.text = gameInfo.Extractions.ToString();
    }

    private void InstantiateTextWithinGridLayoutGroup(GridLayoutGroup layoutGroup, string infoLabel, string infoValue)
    {
        GameObject tempGo = new GameObject();
        GameObject goInfo = Instantiate(tempGo, layoutGroup.transform);
        TextMeshProUGUI infoTxt = goInfo.AddComponent<TextMeshProUGUI>();
        infoTxt.text = infoLabel + " : " + infoValue;
        _gameInfoObjects.Add(goInfo);
    }

    public IEnumerator SceneFader(bool fadeIn, float fadeTimer)
    {
        float alpha = fadeIn ? 0f : 1f;
        float fadeValue = fadeIn ? 1f : 0f;

        if (fadeIn)
        {
            while (alpha <= fadeValue)
            {
                alpha += UpdateTimer(fadeIn, alpha, fadeTimer);
                yield return null;
            }
        }
        else
        {
            while (alpha >= fadeValue)
            {
                alpha += UpdateTimer(fadeIn, alpha, fadeTimer);
                yield return null;
            }
        }
    }

    private float UpdateTimer(bool fadeIn, float alpha, float fadeTimer)
    {
        SetImageColor(alpha);
        return (fadeIn ? +0.1f : -0.1f) * Time.deltaTime * fadeTimer;
    }

    private void SetImageColor(float alpha)
    {
        Color newAlpha = new Color(_sceneFader.color.r, _sceneFader.color.g, _sceneFader.color.b, alpha);
        _sceneFader.color = newAlpha;
    }

    public void ChangeTradeItemsTextVisibility(bool visible)
    {
        _commonTradeItems.gameObject.SetActive(visible);
        _rareTradeItems.gameObject.SetActive(visible);
        _epicTradeItems.gameObject.SetActive(visible);
        _legendaryTradeItems.gameObject.SetActive(visible);
    }

    public void UpdateTradeItemsTextByRarity(Rarity rarity, string nbrItems)
    {
        switch (rarity)
        {
            case Rarity.Common:
                _commonTradeItems.text = nbrItems;
                break;
            case Rarity.Rare:
                _rareTradeItems.text = nbrItems;
                break;
            case Rarity.Epic:
                _epicTradeItems.text = nbrItems;
                break;
            case Rarity.Legendary:
                _legendaryTradeItems.text = nbrItems;
                break;
        }
    }
}
