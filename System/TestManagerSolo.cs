using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestManagerSolo : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private Button _pauseBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _addGripBtn;
    [SerializeField] private Button _spawnMobBtn;
    [SerializeField] private Toggle _toggleColor;
    [SerializeField] private GameObject _colorPanel;
    [SerializeField] private Button _equipPlayerBtn;
    [SerializeField] private Button _addSilencerBtn;
    [SerializeField] private Button _upPlayerLifeBtn;
    [SerializeField] private GameObject _debugPanelUI;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Button _downPlayerLifeBtn;
    [SerializeField] private GameObject _deadBodyPrefab;
    [SerializeField] private GameManagerUI _gameManagerUI;
    [SerializeField] private GameObject _enemyPistolPrefab;
    [SerializeField] private TMP_Dropdown _dropDownWeapons;
    [SerializeField] private TMP_Dropdown _dropDownEnemies;
    [SerializeField] private GameObject _collectablePrefab;
    [SerializeField] private TMP_Dropdown _dropDownSpawners;
    [SerializeField] private GameObject _defaultCharacterPrefab;
    [SerializeField] private TMP_Dropdown _dropDownPlayerWeapons;
    [SerializeField] private List<GameObject> _enemySpawners = new List<GameObject>();

    private GameObject _weaponSelected;
    private PlayerManager _playerManager;
    private CameraManager _cameraManager;
    private List<Toggle> _colorToggles = new List<Toggle>();

    private enum GameState { Lobby, WaveStarted, WaveEnded, GameOver, Pause }

    private void Awake()
    {
        GameData.Instance.CreateBulletPool(_bulletPrefab, 500);

        /*for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(-50, 50), 1f, UnityEngine.Random.Range(-50, 50));
            Rarity rarity = GameData.Instance.GetRandomRarity();

            GameObject collectableGO = Instantiate(_collectablePrefab, position, Quaternion.identity);
            if (collectableGO.TryGetComponent(out Collectable collectable))
            {
                collectable.SetRarity(rarity);
            }
        }*/

        _spawnMobBtn.onClick.AddListener(() =>
        {
            GameObject enemyToSpawn = GameData.Instance.EnemiesData.Find((enemy) => Enum.TryParse("EnemyCultist", out EnemyType result) && result == enemy.EnemyType).EnemyPrefab;
            SpawnMob(enemyToSpawn);
        });

        _resumeBtn.onClick.AddListener(() =>
        {
            ResumeGame();
        });

        _pauseBtn.onClick.AddListener(() =>
        {
            PauseGame();
        });

        _equipPlayerBtn.onClick.AddListener(() =>
        {
            EquipPlayerWithNewWeapon();
        });

        _downPlayerLifeBtn.onClick.AddListener(() =>
        {
            _playerManager.PlayerHit(1);
        });

        _upPlayerLifeBtn.onClick.AddListener(() =>
        {
            _playerManager.PlayerHeal(1);
        });

        _addGripBtn.onClick.AddListener(() =>
        {
        });

        _addSilencerBtn.onClick.AddListener(() =>
        {
        });

        List<GameObject> _weapons = new List<GameObject>();
        foreach (WeaponData weaponData in GameData.Instance.CommonWeaponsSO)
        {
            _weapons.Add(weaponData.WeaponPrefab);
        }

        GetDropDownAndAddOptions(_dropDownEnemies, GameData.Instance.EnemiesData);
    }

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
        _playerManager.gameObject.transform.position = Vector3.zero;
        _playerManager.SetPlayerCharacter(_defaultCharacterPrefab);
    }

    private void UpdatePlayerWeaponsDropDown()
    {
        _dropDownPlayerWeapons.ClearOptions();

        if (_playerManager != null)
        {
            List<string> optionNames = new List<string>();
            if (_playerManager.EquippedWeapon != null)
            {
                optionNames.Add(_playerManager.EquippedWeapon.name);
            }

            _dropDownPlayerWeapons.AddOptions(optionNames);
            if (_dropDownPlayerWeapons.options.Count > 0) _weaponSelected = _playerManager.EquippedWeapon;
        }
    }

    private void GetDropDownAndAddOptions(TMP_Dropdown dropwDown, List<EnemyData> options)
    {
        List<string> optionNames = new List<string>();
        foreach (EnemyData option in options)
        {
            optionNames.Add(option.name);
        }

        dropwDown.AddOptions(optionNames);
    }

    private void EquipPlayerWithNewWeapon()
    {
        if (_playerManager != null)
        {
            int index = GameData.Instance.CommonWeaponsSO.FindIndex((weapon) => Enum.TryParse("Pistol", out WeaponType result) && result == weapon.WeaponType);

            _playerManager.EquipNewWeapon(GameData.Instance.CommonWeaponsSO[index].WeaponPrefab);
        }
    }

    private void SpawnMob(GameObject enemyGameObject)
    {
        GameObject enemy = Instantiate(enemyGameObject, new Vector3(0, 0, 0), Quaternion.identity);
        if (enemy != null)
        {
            enemy.SetActive(true);
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }
}
