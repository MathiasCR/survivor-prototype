using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("Main Camera Transform Initial in Lobby")]
    [SerializeField] private Vector3 _initLobbyCamPosition;
    [SerializeField] private Vector3 _initLobbyCamRotation;

    [SerializeField] private List<PlayerData> _charactersData;

    [SerializeField] private List<Biome> _biomes;

    [SerializeField] private List<WeaponModData> _commonModsSO;
    [SerializeField] private List<WeaponModData> _rareModsSO;
    [SerializeField] private List<WeaponModData> _epicModsSO;
    [SerializeField] private List<WeaponModData> _legendaryModsSO;

    [SerializeField] private List<WeaponData> _commonWeaponsSO;
    [SerializeField] private List<WeaponData> _rareWeaponsSO;
    [SerializeField] private List<WeaponData> _epicWeaponsSO;
    [SerializeField] private List<WeaponData> _legendaryWeaponsSO;

    [SerializeField] private List<GazFilterData> _commonGazFilterSO;
    [SerializeField] private List<GazFilterData> _rareGazFilterSO;
    [SerializeField] private List<GazFilterData> _epicGazFilterSO;
    [SerializeField] private List<GazFilterData> _legendaryGazFilterSO;

    [SerializeField] private Material _commonItemDrop;
    [SerializeField] private Material _rareItemDrop;
    [SerializeField] private Material _epicItemDrop;
    [SerializeField] private Material _legendaryItemDrop;

    [SerializeField] private List<EnemyData> _enemiesData;

    public static GameData Instance { get; private set; }

    public float ChunkOffSet = 70f;

    private GameObjectPool _enemyPool;
    private GameObjectPool _bulletPool;
    private PlayerManager _playerManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
    }

    public PlayerData GetCharacterData(string characterName)
    {
        return _charactersData.Find((character) =>
        {
            return character.name == characterName;
        });
    }

    public void CreateBulletPool(GameObject bulletPrefab, int nbr)
    {
        _bulletPool = new GameObjectPool(bulletPrefab, nbr);
    }

    public void CreateEnemyPool(GameObject enemyPrefab, int nbr)
    {
        _enemyPool = new GameObjectPool(enemyPrefab, nbr);
    }

    public Rarity GetRandomRarity()
    {
        int random = Random.Range(0, 100);
        Debug.Log($"random : {random} - luck : {_playerManager.Luck}");
        random += random * -(_playerManager.Luck / 100);
        Debug.Log($"new random : {random}");

        if (random < 5)
        {
            return Rarity.Legendary;
        }

        if (random < 15)
        {
            return Rarity.Epic;
        }

        if (random < 40)
        {
            return Rarity.Rare;
        }

        return Rarity.Common;
    }

    public GameObject GetEnemyPrefabByTypeAndTier(EnemyType enemyType, EnemyTier enemyTier)
    {
        EnemyData enemy = _enemiesData.Find((enemy) => enemy.EnemyType == enemyType && enemy.EnemyTier == enemyTier);

        if (enemy != null)
        {
            return enemy.EnemyPrefab;
        }

        Debug.LogError($"No Enemy data found with enemyType {enemyType} and enemyTier {enemyTier} °_°");
        return null;
    }

    public Material GetMaterialByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonItemDrop;
            case Rarity.Rare:
                return _rareItemDrop;
            case Rarity.Epic:
                return _epicItemDrop;
            case Rarity.Legendary:
                return _legendaryItemDrop;
            default:
                return _commonItemDrop;
        }
    }

    public List<WeaponData> GetWeaponDataByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonWeaponsSO;
            case Rarity.Rare:
                return _rareWeaponsSO;
            case Rarity.Epic:
                return _epicWeaponsSO;
            case Rarity.Legendary:
                return _legendaryWeaponsSO;
            default:
                return _commonWeaponsSO;
        }
    }

    public List<WeaponModData> GetWeaponModDataByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonModsSO;
            case Rarity.Rare:
                return _rareModsSO;
            case Rarity.Epic:
                return _epicModsSO;
            case Rarity.Legendary:
                return _legendaryModsSO;
            default:
                return _commonModsSO;
        }
    }

    public List<GazFilterData> GetGazFilterDataByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return _commonGazFilterSO;
            case Rarity.Rare:
                return _rareGazFilterSO;
            case Rarity.Epic:
                return _epicGazFilterSO;
            case Rarity.Legendary:
                return _legendaryGazFilterSO;
            default:
                return _commonGazFilterSO;
        }
    }

    public List<WeaponModData> GetWeaponModDataByRarityForPlayerWeapons(Rarity rarity)
    {
        PlayerManager playerManager = GameManager.Instance.GetPlayerManager();

        if (playerManager == null) return new List<WeaponModData>();

        if (!playerManager.EquippedWeapon.TryGetComponent(out WeaponManager weaponManager)) return new List<WeaponModData>();

        List<WeaponModData> mods = new List<WeaponModData>();
        List<WeaponModData> availableMods = new List<WeaponModData>();

        switch (rarity)
        {
            case Rarity.Common:
                mods = _commonModsSO;
                break;
            case Rarity.Rare:
                mods = _rareModsSO;
                break;
            case Rarity.Epic:
                mods = _epicModsSO;
                break;
            case Rarity.Legendary:
                mods = _legendaryModsSO;
                break;
        }

        foreach (WeaponModData mod in mods)
        {
            if (mod.CompatibleWeaponTypes.Contains(weaponManager.WeaponType))
            {
                availableMods.Add(mod);
            }
        }

        return availableMods;
    }

    public List<Biome> Biomes
    {
        get => _biomes;
    }

    public List<EnemyData> EnemiesData
    {
        get => _enemiesData;
    }

    public List<WeaponData> CommonWeaponsSO
    {
        get => _commonWeaponsSO;
    }

    public List<WeaponData> RareWeaponsSO
    {
        get => _rareWeaponsSO;
    }

    public List<WeaponData> EpicWeaponsSO
    {
        get => _epicWeaponsSO;
    }

    public List<WeaponData> LegendaryWeaponsSO
    {
        get => _legendaryWeaponsSO;
    }

    public List<WeaponModData> CommonModsSO
    {
        get => _commonModsSO;
    }

    public List<WeaponModData> RareModsSO
    {
        get => _rareModsSO;
    }

    public List<WeaponModData> EpicModsSO
    {
        get => _epicModsSO;
    }

    public List<WeaponModData> LegendaryModsSO
    {
        get => _legendaryModsSO;
    }

    public GameObjectPool BulletPool
    {
        get => _bulletPool;
    }

    public GameObjectPool EnemyPool
    {
        get => _enemyPool;
    }

    public Vector3 InitLobbyCamPosition
    {
        get => _initLobbyCamPosition;
    }

    public Vector3 InitLobbyCamRotation
    {
        get => _initLobbyCamRotation;
    }
}
