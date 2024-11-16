using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyScriptableObject", order = 2)]
public class EnemyData : ScriptableObject
{
    [Header("Base Enemy Settings")]
    public int EnemyLife;
    public int EnemyDamage;
    public EnemyType EnemyType;
    public EnemyTier EnemyTier;
    public GameObject EnemyPrefab;

    [Header("Enemy Attack Settings")]
    public float AttackRange;
    public float AttackSpeed;
    public float AimingSpeed;

    [Header("Movement Enemy Settings")]
    public float MoveSpeed;

    [Header("Item Drop Enemy Settings")]
    public int ItemDropRate;
    public int CommonDropRate;
    public int RareDropRate;
    public int EpicDropRate;
    public int LegendaryDropRate;
}

public enum EnemyType
{
    EnemyCultist
}

public enum EnemyTier
{
    Tier1,
    Tier2,
    Tier3,
}
