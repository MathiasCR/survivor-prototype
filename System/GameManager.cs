using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameManagerUI _gui;
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 _playerOriginPos;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private List<Level> _streetLevels;
    [SerializeField] private GameObject _tradeItemPrefab;
    [SerializeField] private GameObject _extractionPrefab;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private GameObject _defaultCharacterPrefab;

    public event Action OnSceneChange;

    public enum GameState { MainMenu, Lobby, LevelStarted, LevelOngoing, PlayerDead, PlayerExtracted, StatsShown, Shop }

    public GameState PreviousGameState;
    public GameState CurrentGameState;

    private int _levelCount = 0;
    private Level _currentLevel;
    private PlayerGUI _playerGUI;
    private float _levelTimer = 0f;
    private BiomeType _selectedBiome;
    private PlayerAction _playerAction;
    private float _lobbyFadeTimer = 4f;
    private float _defaultFadeTimer = 5f;
    private PlayerManager _playerManager;
    private float _spawnEnemiesDelay = 5f;
    private PlayerMovement _playerMovement;
    private Dictionary<Rarity, int> _tradeItemsCollected;
    private GameInfo _currentRunGameInfos = new GameInfo();
    private GameInfo _currentLevelGameInfos = new GameInfo();
    private List<GameObject> _enemiesList = new List<GameObject>();
    private List<CrateBehaviour> _spawnerCrateBehaviours = new List<CrateBehaviour>();
    private Dictionary<GameObject, Chunk> _generatedMap = new Dictionary<GameObject, Chunk>();

    private bool IsInLevelOrShop => CurrentGameState == GameState.LevelOngoing || CurrentGameState == GameState.Shop;

    private void Awake()
    {
        Instance = this;

        if (_player.TryGetComponent(out PlayerAction playerAction))
        {
            _playerAction = playerAction;
            playerAction.OnEscapePressed += OnEscapePressed;
        }

        if (_player.TryGetComponent(out PlayerMovement playerMovement))
        {
            _playerMovement = playerMovement;
        }

        if (_player.TryGetComponent(out PlayerManager playerManager))
        {
            _playerManager = playerManager;
        }

        if (_player.TryGetComponent(out PlayerGUI playerGUI))
        {
            _playerGUI = playerGUI;
        }
    }

    private void Start()
    {
        GameData.Instance.CreateBulletPool(_bulletPrefab, 100);
    }

    private void Update()
    {
        if (CurrentGameState == GameState.LevelOngoing && _levelTimer > 0)
        {
            // Timer Gestion
            _levelTimer -= Time.deltaTime;
            _currentLevelGameInfos.TimeElapsed += Time.deltaTime;
            _levelTimer = _levelTimer < 0 ? 0 : _levelTimer;
            _gui.UpdateRemainingTime(_levelTimer);
        }
        else if (CurrentGameState == GameState.LevelOngoing && _levelTimer == 0)
        {
            _playerManager.PlayerHit(1);
        }
    }

    private void ChangeGameState(GameState newValue)
    {
        PreviousGameState = CurrentGameState;
        CurrentGameState = newValue;

        if (PreviousGameState == GameState.Lobby) LobbyManager.Instance.OnLobbyLeave();
        if (PreviousGameState == GameState.Shop) ShopManager.Instance.OnShopLeave();

        switch (newValue)
        {
            case GameState.MainMenu:
                StartCoroutine(LoadMainMenuScene());
                break;
            case GameState.Lobby:
                StartCoroutine(LoadLobbyScene());
                break;
            case GameState.LevelStarted:
                StartCoroutine(LoadLevelScene());
                break;
            case GameState.LevelOngoing:
                StartCoroutine("SpawnEnemies");
                break;
            case GameState.PlayerDead:
                _gui.ChangeRemainingTimeVisibilty(false);
                _playerManager.ChangeHealthVisibility(false);
                StopCoroutine("SpawnEnemies");
                ChangeGameState(GameState.StatsShown);
                break;
            case GameState.PlayerExtracted:
                StopCoroutine("SpawnEnemies");
                _levelCount++;
                ChangeGameState(GameState.StatsShown);
                break;
            case GameState.StatsShown:
                switch (PreviousGameState)
                {
                    case GameState.PlayerDead:
                    case GameState.Shop:
                        AddLevelStatToRun();
                        ShowRunStats();
                        break;
                    case GameState.PlayerExtracted:
                        ShowLevelStats();
                        break;
                }
                break;
            case GameState.Shop:
                StartCoroutine(LoadShopScene());
                break;
            default: break;
        }
    }

    private IEnumerator LoadMainMenuScene()
    {
        // Fade in
        yield return _gui.SceneFader(true, _defaultFadeTimer);

        // Settings before loading the scene
        DestroyMap();
        _player.SetActive(false);
        _gui.ChangeRemainingTimeVisibilty(false);
        _gui.ChangeTradeItemsTextVisibility(false);

        // Load Main Menu Scene
        var asyncLoadLevel = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // Settings before fade out and after loading the scene


        // Fade out
        yield return _gui.SceneFader(false, _defaultFadeTimer);

        // Settings after fade out
        ResumeGame();
        _playerManager.IsInMenu = false;
    }

    private IEnumerator LoadLobbyScene()
    {
        // Fade in
        yield return _gui.SceneFader(true, _defaultFadeTimer);

        // Settings before loading the scene
        DestroyMap();
        _playerManager.ResetPlayerLife();
        _playerManager.RemoveEquippedWeapon();
        _currentRunGameInfos = new GameInfo();
        _gui.ChangeRemainingTimeVisibilty(false);
        _gui.ChangeGameInfosPanelVisibility(false);
        _gui.ChangeTradeItemsTextVisibility(false);
        _playerManager.ChangeHealthVisibility(false);

        _tradeItemsCollected = new Dictionary<Rarity, int>
        {
            { Rarity.Common, 0 },
            { Rarity.Rare, 0 },
            { Rarity.Epic, 0 },
            { Rarity.Legendary, 0 }
        };

        // Load Lobby Scene
        var asyncLoadLevel = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // Settings before fade out and after loading the scene
        _player.transform.position = _playerOriginPos;

        if (_player.activeInHierarchy)
        {
            OnSceneChange?.Invoke();
            _navMeshSurface.BuildNavMesh();
        }
        else
        {
            _navMeshSurface.BuildNavMesh();
            _player.SetActive(true);
        }

        // Fade out
        yield return _gui.SceneFader(false, _lobbyFadeTimer);

        // Settings after fade out
        _levelCount = 0;
        _playerManager.IsInMenu = false;
    }

    private IEnumerator LoadShopScene()
    {
        // Fade in
        yield return _gui.SceneFader(true, _defaultFadeTimer);

        // Settings before loading the scene
        DestroyMap();
        _gui.ChangeGameInfosPanelVisibility(false);

        // Load Shop Scene
        var asyncLoadLevel = SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // Settings before fade out and after loading the scene
        OnSceneChange?.Invoke();
        _navMeshSurface.BuildNavMesh();

        // Fade out
        yield return _gui.SceneFader(false, _lobbyFadeTimer);

        // Settings after fade out
        _playerManager.IsInMenu = false;
    }

    private IEnumerator LoadLevelScene()
    {
        // Fade in
        yield return _gui.SceneFader(true, _defaultFadeTimer);

        // Settings before loading the scene
        GetCurrentLevel();

        // Load Level Scene
        var asyncLoadLevel = SceneManager.LoadSceneAsync("Level", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }


        // Settings before fade out and after loading the scene
        // Generate Level Map
        _generatedMap = MapGenerator.Instance.StartMapGeneration(_selectedBiome);
        _navMeshSurface.BuildNavMesh();
        PopulateMap();

        // Fade out
        yield return _gui.SceneFader(false, _lobbyFadeTimer);

        // Settings after fade out
        _gui.ChangeRemainingTimeVisibilty(true);
        _gui.ChangeTradeItemsTextVisibility(true);
        _playerManager.ChangeHealthVisibility(true);
        _playerManager.IsInMenu = false;

        ChangeGameState(GameState.LevelOngoing);
    }

    private void PopulateMap()
    {
        // Spawn Player / Extraction / Lootable
        GameObject centerChunk = _generatedMap.Where((chunk) => chunk.Key.transform.position == Vector3.zero).FirstOrDefault().Key;
        if (centerChunk.TryGetComponent(out ChunkData centerChunkData))
        {
            _player.transform.position = centerChunkData.PlayerSpawner.transform.position;
        }

        List<GameObject> extractions = new List<GameObject>();
        List<GameObject> crates = new List<GameObject>();
        foreach (GameObject chunkGo in _generatedMap.Keys)
        {
            if (chunkGo.TryGetComponent(out ChunkData chunkData))
            {
                if ((chunkGo.transform.position.x == GameData.Instance.ChunkOffSet * 2 || chunkGo.transform.position.z == GameData.Instance.ChunkOffSet * 2)
                        && chunkData.Extraction != null)
                {
                    extractions.Add(chunkData.Extraction);
                }

                if (chunkData.Crates.Count > 0)
                {
                    crates.AddRange(chunkData.Crates);
                }
            }
        }

        if (crates.Count > 0)
        {
            List<int> randomIndexes = Utils.GenerateRandom(_currentLevel.NbrCrates, 0, crates.Count);

            foreach (int index in randomIndexes)
            {
                crates[index].SetActive(true);
                if (crates[index].TryGetComponent(out CrateBehaviour crateBehaviour))
                {
                    _spawnerCrateBehaviours.Add(crateBehaviour);
                    crateBehaviour.OnPlayerGetFilterGaz += OnPlayerGetGazFilter;
                    crateBehaviour.OnPlayerGetMod += OnPlayerGetMod;
                }
            }
        }

        extractions[UnityEngine.Random.Range(0, extractions.Count)].SetActive(true);
    }

    private void GetCurrentLevel()
    {
        switch (_selectedBiome)
        {
            case BiomeType.Street:
                _currentLevel = _streetLevels[_levelCount];
                break;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_spawnEnemiesDelay);

        while (CurrentGameState == GameState.LevelOngoing)
        {
            GameObject currentChunk = GetCurrentPositionChunk();

            if (currentChunk == null) yield return null;

            List<GameObject> spawners = MapGenerator.Instance.GetAccessibleNearEnemySpawners(currentChunk);
            if (spawners.Count > 0 && _enemiesList.Count < 100)
            {
                List<int> randomIndexes = Utils.GenerateRandom(spawners.Count / 2, 0, spawners.Count);

                foreach (int index in randomIndexes)
                {
                    GameObject enemy = GetRandomEnemyLevel();

                    if (enemy == null) yield break;

                    _enemiesList.Add(Instantiate(enemy, spawners[index].transform.position, Quaternion.identity));
                }
            }

            yield return new WaitForSeconds(_spawnEnemiesDelay);
        }
    }

    private GameObject GetCurrentPositionChunk()
    {
        Vector3 playerPos = _player.transform.position;
        Ray ray = new Ray(playerPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            GameObject chunkGo = raycastHit.collider.transform.root.gameObject;
            return chunkGo;
        }

        Debug.LogError("No chunk found under the player °_°");
        return null;
    }

    private GameObject GetRandomEnemyLevel()
    {
        int randomSpawnRate = new System.Random().Next(1, 100);

        foreach (LevelEnemy enemy in _currentLevel.LevelEnemies)
        {
            if (randomSpawnRate < enemy.SpawnRate)
            {
                return GameData.Instance.GetEnemyPrefabByTypeAndTier(enemy.Enemy, _currentLevel.LevelEnemyTier);
            }
        }

        Debug.LogError($"No enemy found for this spawn rate {randomSpawnRate} °_°");
        return null;
    }

    private void DestroyMap()
    {
        foreach (GameObject chunkGo in _generatedMap.Keys)
        {
            Destroy(chunkGo);
        }

        foreach (CrateBehaviour crateBehaviour in _spawnerCrateBehaviours)
        {
            crateBehaviour.OnPlayerGetFilterGaz -= OnPlayerGetGazFilter;
            crateBehaviour.OnPlayerGetMod -= OnPlayerGetMod;
        }

        _generatedMap.Clear();
    }

    public void OnStartGame()
    {
        _playerManager.gameObject.transform.position = Vector3.zero;
        _playerManager.SetPlayerCharacter(_defaultCharacterPrefab);
        ChangeGameState(GameState.Lobby);
    }

    public void OnExtraction()
    {
        PauseGame();
        _currentLevelGameInfos.Extractions++;
        ChangeGameState(GameState.PlayerExtracted);
    }

    public void OnEnemyDead(EnemyManager enemy)
    {
        int dropRoll = UnityEngine.Random.Range(0, 100);
        if (dropRoll < enemy.ItemDropRate)
        {
            SpawnTradeItem(EnemyDropRandomItem(enemy), enemy.transform.position);
        }

        if (_currentLevelGameInfos.killsByEnemyType.ContainsKey(enemy.EnemyType))
        {
            _currentLevelGameInfos.killsByEnemyType[enemy.EnemyType] += 1;
        }
        else
        {
            _currentLevelGameInfos.killsByEnemyType.Add(enemy.EnemyType, 1);
        }
    }

    public void OnPlayerChooseNextLevel()
    {
        _playerManager.IsInMenu = true;
        _gui.ShowNextLevelPanelVisibility();
        _gui.OnNextLevelSelected += OnNextLevelSelected;
    }

    private void OnNextLevelSelected(int biomeIndex)
    {
        _gui.OnNextLevelSelected -= OnNextLevelSelected;
        _selectedBiome = GameData.Instance.Biomes[biomeIndex].BiomeType;
        _levelTimer = _playerManager.DefaultFilterTimer;
        ChangeGameState(GameState.LevelStarted);
    }

    public void OnPlayerDead()
    {
        PauseGame();
        ChangeGameState(GameState.PlayerDead);
    }

    public void OnPlayerReturnToLobby()
    {
        PauseGame();
        ChangeGameState(GameState.StatsShown);
    }

    public void OnPlayerGoesToNextLevel()
    {
        _playerManager.IsInMenu = true;
        ChangeGameState(GameState.LevelStarted);
    }

    private Rarity EnemyDropRandomItem(EnemyManager enemyManager)
    {
        int dropRoll = UnityEngine.Random.Range(0, 100);
        if (dropRoll < enemyManager.LegendaryDropRate)
        {
            return Rarity.Legendary;
        }

        if (dropRoll < enemyManager.EpicDropRate)
        {
            return Rarity.Epic;
        }

        if (dropRoll < enemyManager.RareDropRate)
        {
            return Rarity.Rare;
        }

        return Rarity.Common;
    }

    public void SpawnTradeItem(Rarity rarity, Vector3 position)
    {
        GameObject tradeItemGO = Instantiate(_tradeItemPrefab, position, Quaternion.identity);
        if (tradeItemGO.TryGetComponent(out Collectable collectable))
        {
            collectable.SetRarity(rarity);
        }
    }

    public void OnPlayerGetMod(WeaponMod previousMod, WeaponMod mod)
    {
        Debug.Log("Player got mod: " + mod.ModData.name);
        if (previousMod != null)
        {
            Debug.Log("Replacing mod: " + previousMod.ModData.name);
        }
        _playerManager.AddModToEquippedWeapon(previousMod, mod);
        AddCollectedItemToGameInfos(LootType.Mod);
    }

    public void OnPlayerGetGazFilter(GazFilterData gazFilterData)
    {
        _levelTimer += gazFilterData.FilterTimerBonus;
        _gui.UpdateRemainingTime(_levelTimer);
        AddCollectedItemToGameInfos(LootType.Filter);
    }

    public void OnTradeItemCollected(Rarity rarity)
    {
        _tradeItemsCollected[rarity]++;
        _gui.UpdateTradeItemsTextByRarity(rarity, _tradeItemsCollected[rarity].ToString());

        switch (rarity)
        {
            case Rarity.Common:
                AddCollectedItemToGameInfos(LootType.ToiletPaper);
                break;
            case Rarity.Rare:
                AddCollectedItemToGameInfos(LootType.Cigaret);
                break;
            case Rarity.Epic:
                AddCollectedItemToGameInfos(LootType.Vodka);
                break;
            case Rarity.Legendary:
                AddCollectedItemToGameInfos(LootType.PornMagazine);
                break;
        }
    }

    private void AddCollectedItemToGameInfos(LootType lootType)
    {
        if (_currentLevelGameInfos.lootsByLootType.ContainsKey(lootType))
        {
            _currentLevelGameInfos.lootsByLootType[lootType] += 1;
        }
        else
        {
            _currentLevelGameInfos.lootsByLootType.Add(lootType, 1);
        }
    }

    public int GetTradeItemByRarity(Rarity rarity)
    {
        return _tradeItemsCollected[rarity];
    }

    public void RemoveTradeItemByRarity(Rarity rarity)
    {
        _tradeItemsCollected[rarity]--;
        _gui.UpdateTradeItemsTextByRarity(rarity, _tradeItemsCollected[rarity].ToString());
    }

    private void OnEscapePressed()
    {
        switch (CurrentGameState)
        {
            case GameState.Lobby:
            case GameState.LevelOngoing:
                _playerManager.IsInMenu = !_playerManager.IsInMenu;
                _gui.ChangeOptionsMenuPanelVisibility(_playerManager.IsInMenu, IsInLevelOrShop);

                if (_playerManager.IsInMenu)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }

                break;
            case GameState.StatsShown:
                OnContinueFromStatsPanel();
                break;
        }
    }

    private void ShowLevelStats()
    {
        _playerManager.IsInMenu = true;

        //Affiche les stats du level.
        _gui.ChangeGameInfosPanelVisibility(true);
        _gui.SetGameInformationsData(_currentLevelGameInfos);
    }

    private void AddLevelStatToRun()
    {
        //Ajoute les stats du level au run
        _currentRunGameInfos = _currentRunGameInfos + _currentLevelGameInfos;

        //Reset les stats du level
        _currentLevelGameInfos = new GameInfo();
    }

    private void ShowRunStats()
    {
        _playerManager.IsInMenu = true;

        //Affiche les stats du level.
        _gui.ChangeGameInfosPanelVisibility(true);
        _gui.SetGameInformationsData(_currentRunGameInfos);
    }

    public void OnContinueFromStatsPanel()
    {
        ResumeGame();
        switch (PreviousGameState)
        {
            case GameState.PlayerDead:
            case GameState.Shop:
                ChangeGameState(GameState.Lobby);
                break;
            case GameState.PlayerExtracted:
                ChangeGameState(GameState.Shop);
                break;
            default:
                Debug.LogError("Wrong Game State to continue on");
                break;
        }
    }

    public void OnOptionsMenuBack()
    {
        // Close options menu and player can move
        _gui.ChangeOptionsMenuPanelVisibility(false, IsInLevelOrShop);
        _playerManager.IsInMenu = false;
        ResumeGame();
    }

    public void OnOptionsMenuToLobby()
    {
        // Return to lobby
        _gui.ChangeOptionsMenuPanelVisibility(false, IsInLevelOrShop);
        ResumeGame();
        ChangeGameState(GameState.Lobby);
    }

    public void OnOptionsMenuToMainMenu()
    {
        // Return to main menu
        _gui.ChangeOptionsMenuPanelVisibility(false, IsInLevelOrShop);
        ResumeGame();
        ChangeGameState(GameState.MainMenu);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public PlayerGUI GetPlayerGUI()
    {
        return _playerGUI;
    }

    public PlayerManager GetPlayerManager()
    {
        return _playerManager;
    }

    public PlayerAction GetPlayerAction()
    {
        return _playerAction;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return _playerMovement;
    }
}
