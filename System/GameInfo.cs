using System.Collections.Generic;

public class GameInfo
{
    public int TotalKills;
    public int Extractions;
    public float TimeElapsed;
    public Dictionary<LootType, int> lootsByLootType;
    public Dictionary<EnemyType, int> killsByEnemyType;

    public GameInfo()
    {
        TotalKills = 0;
        Extractions = 0;
        TimeElapsed = 0;
        lootsByLootType = new Dictionary<LootType, int>();
        killsByEnemyType = new Dictionary<EnemyType, int>();
    }

    public static GameInfo operator +(GameInfo left, GameInfo right)
    {
        Dictionary<EnemyType, int> newKillsByEnemyType = Utils.ConcatenateTwoDictionaries<EnemyType, int>(left.killsByEnemyType, right.killsByEnemyType);

        Dictionary<LootType, int> newLootsByLootType = Utils.ConcatenateTwoDictionaries<LootType, int>(left.lootsByLootType, right.lootsByLootType);

        return new GameInfo()
        {
            TotalKills = left.TotalKills + right.TotalKills,
            Extractions = left.Extractions + right.Extractions,
            TimeElapsed = left.TimeElapsed + right.TimeElapsed,
            lootsByLootType = newLootsByLootType,
            killsByEnemyType = newKillsByEnemyType,
        };
    }
}
