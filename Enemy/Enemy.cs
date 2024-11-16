using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private EnemyGUI _enemyGUI;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private EnemyAction _enemyAction;
    [SerializeField] private BoxCollider _enemyCollider;

    [Header("Base Enemy Settings Read Only")]
    [SerializeField] public int EnemyLife;
    [SerializeField] public int EnemyDamage;
    [SerializeField] public EnemyType EnemyType;
    [SerializeField] public EnemyTier EnemyTier;
    [SerializeField] public GameObject EnemyPrefab;

    [Header("Enemy Attack Settings Read Only")]
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackSpeed;
    [SerializeField] public float AimingSpeed;

    [Header("Movement Enemy Settings Read Only")]
    [SerializeField] public float MoveSpeed;

    [Header("Item Drop Enemy Settings Read Only")]
    [SerializeField] public int ItemDropRate;
    [SerializeField] public int CommonDropRate;
    [SerializeField] public int RareDropRate;
    [SerializeField] public int EpicDropRate;
    [SerializeField] public int LegendaryDropRate;

    public event Action<float> OnEnemyHit;
    public event Action OnEnemyDeath;

    private int _currentEnemyLife;

    private void Awake()
    {
        EnemyLife = _enemyData.EnemyLife;
        EnemyDamage = _enemyData.EnemyDamage;
        EnemyType = _enemyData.EnemyType;
        EnemyTier = _enemyData.EnemyTier;
        MoveSpeed = _enemyData.MoveSpeed;
        AttackSpeed = _enemyData.AttackSpeed;
        AttackRange = _enemyData.AttackRange;
        AimingSpeed = _enemyData.AimingSpeed;
        ItemDropRate = _enemyData.ItemDropRate;
        CommonDropRate = _enemyData.CommonDropRate;
        RareDropRate = _enemyData.RareDropRate;
        EpicDropRate = _enemyData.EpicDropRate;
        LegendaryDropRate = _enemyData.LegendaryDropRate;

        _currentEnemyLife = _enemyData.EnemyLife;
        _enemyGUI.SetHealthBar(_enemyData.EnemyLife);
    }

    public void TakeDamage(int damage, float stunTimer, bool isHeadShot)
    {
        _currentEnemyLife -= damage;

        if (_currentEnemyLife <= 0)
        {
            OnEnemyDeath?.Invoke();
            _enemyGUI.HideHealthBar();
            _enemyCollider.enabled = false;
        }
        else
        {
            _enemyGUI.UpdateHealthBar(_currentEnemyLife);
            OnEnemyHit?.Invoke(stunTimer);
        }
    }
}
