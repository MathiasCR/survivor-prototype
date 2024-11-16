using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject", order = 7)]
public class Level : ScriptableObject
{
    [SerializeField] public int LevelId;
    [SerializeField] public BiomeType LevelBiome;
    [SerializeField] public EnemyTier LevelEnemyTier;
    [SerializeField][Range(0, 10)] public int NbrCrates;
    [SerializeField][Range(0, 10)] public int NbrAnomalies;
    [SerializeField] public List<LevelEnemy> LevelEnemies;
}

[Serializable]
public struct LevelEnemy
{
    public int SpawnRate;
    public EnemyType Enemy;
}