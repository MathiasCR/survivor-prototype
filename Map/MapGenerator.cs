using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<Chunk> _streetChunks;
    [SerializeField] private Chunk _streetDefaultChunk;

    public static MapGenerator Instance { get; private set; }

    private Chunk _defaultChunk;
    private float _chunkOffSet;
    private List<Chunk> _biomeChunks = new List<Chunk>();
    private Dictionary<GameObject, Chunk> _generatedMap = new Dictionary<GameObject, Chunk>();
    private Dictionary<Vector3, ChunkToGenerate> _chunkCoordonates = new Dictionary<Vector3, ChunkToGenerate>();
    private Dictionary<Vector3, Direction> _straightNeighboursChunkPos = new Dictionary<Vector3, Direction>
    {
        { new Vector3(70, 0, 0), Direction.Right },
        { new Vector3(-70, 0, 0), Direction.Left },
        { new Vector3(0, 0, 70), Direction.Up },
        { new Vector3(0, 0, -70), Direction.Down },
    };

    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<GameObject, Chunk> StartMapGeneration(BiomeType biome)
    {
        _generatedMap.Clear();
        _chunkCoordonates.Clear();
        _chunkOffSet = GameData.Instance.ChunkOffSet;
        switch (biome)
        {
            case BiomeType.Street:
                _biomeChunks = _streetChunks;
                _defaultChunk = _streetDefaultChunk;
                GenerateMap();
                break;
        }

        return _generatedMap;
    }

    private void GenerateMap()
    {
        for (int x = -2; x < 3; x++)
        {
            if (x % 2 == 0)
            {
                for (int z = -2; z < 3; z++)
                {
                    GenerateMapLayout(x, z);
                }
            }
            else
            {
                for (int z = 2; z > -3; z--)
                {
                    GenerateMapLayout(x, z);
                }
            }
        }
    }

    private void GenerateMapLayout(int x, int z)
    {
        Vector3 position = new Vector3(70 * x, 0, 70 * z);

        List<Direction> forbiddenDirections = new List<Direction>();

        if (z == -2) forbiddenDirections.Add(Direction.Down);

        if (z == 2) forbiddenDirections.Add(Direction.Up);

        if (x == -2) forbiddenDirections.Add(Direction.Left);

        if (x == 2) forbiddenDirections.Add(Direction.Right);

        ChunkToGenerate chunkToGenerate = GetChunkToGenerate(forbiddenDirections, position);
        _chunkCoordonates.Add(position, chunkToGenerate);
        _generatedMap.Add(Instantiate(chunkToGenerate.Chunk.ChunkPrefab, position, Quaternion.Euler(chunkToGenerate.ChunkRotation.Rotation)), chunkToGenerate.Chunk);
    }

    private ChunkToGenerate GetChunkToGenerate(List<Direction> forbiddenDirections, Vector3 position)
    {
        List<ChunkToGenerate> availableChunks = new List<ChunkToGenerate>();

        foreach (Chunk chunk in _biomeChunks)
        {
            List<ChunkRotation> availableRotations = GetChunkRotationsAvailable(chunk, forbiddenDirections);
            foreach (ChunkRotation rotation in availableRotations)
            {
                availableChunks.Add(new ChunkToGenerate(chunk, rotation));
            }
        }

        List<ChunkToGenerate> fittableChunks = new List<ChunkToGenerate>();

        foreach (ChunkToGenerate chunkToGenerate in availableChunks)
        {
            if (CanChunkFit(chunkToGenerate, position))
            {
                fittableChunks.Add(chunkToGenerate);
            }
        }

        if (fittableChunks.Count == 0) return new ChunkToGenerate(_defaultChunk, _defaultChunk.ChunkRotations[0]);

        ChunkToGenerate randomChunk = fittableChunks[Random.Range(0, fittableChunks.Count)];

        return randomChunk;
    }

    private bool CanChunkFit(ChunkToGenerate chunkToGenerate, Vector3 position)
    {
        int nbrDirectionOccupied = 0;

        if (_chunkCoordonates.TryGetValue(position + new Vector3(0, 0, -_chunkOffSet), out ChunkToGenerate downChunk))
        {
            if (chunkToGenerate.ChunkRotation.Directions.Contains(Direction.Down) != downChunk.ChunkRotation.IsDownRotation)
            {
                return false;
            }

            if (downChunk.ChunkRotation.IsDownRotation)
            {
                nbrDirectionOccupied++;
            }
        }

        if (_chunkCoordonates.TryGetValue(position + new Vector3(0, 0, _chunkOffSet), out ChunkToGenerate upChunk))
        {
            if (chunkToGenerate.ChunkRotation.Directions.Contains(Direction.Up) != upChunk.ChunkRotation.IsUpRotation)
            {
                return false;
            }

            if (upChunk.ChunkRotation.IsUpRotation)
            {
                nbrDirectionOccupied++;
            }
        }

        if (_chunkCoordonates.TryGetValue(position + new Vector3(_chunkOffSet, 0, 0), out ChunkToGenerate rightChunk))
        {
            if (chunkToGenerate.ChunkRotation.Directions.Contains(Direction.Right) != rightChunk.ChunkRotation.IsRightRotation)
            {
                return false;
            }

            if (rightChunk.ChunkRotation.IsRightRotation)
            {
                nbrDirectionOccupied++;
            }
        }

        if (_chunkCoordonates.TryGetValue(position + new Vector3(-_chunkOffSet, 0, 0), out ChunkToGenerate leftChunk))
        {
            if (chunkToGenerate.ChunkRotation.Directions.Contains(Direction.Left) != leftChunk.ChunkRotation.IsLeftRotation)
            {
                return false;
            }

            if (leftChunk.ChunkRotation.IsLeftRotation)
            {
                nbrDirectionOccupied++;
            }
        }

        if (nbrDirectionOccupied >= chunkToGenerate.ChunkRotation.Directions.Count) return false;

        return true;
    }

    private List<ChunkRotation> GetChunkRotationsAvailable(Chunk chunk, List<Direction> forbiddenDirections)
    {
        List<ChunkRotation> availableRotations = new List<ChunkRotation>();

        foreach (ChunkRotation rotation in chunk.ChunkRotations)
        {
            if (forbiddenDirections.Intersect(rotation.Directions).Count() == 0)
            {
                availableRotations.Add(rotation);
            }
        }

        return availableRotations;
    }

    public List<GameObject> GetAccessibleNearEnemySpawners(GameObject chunk)
    {
        List<GameObject> nearAvailableEnemySpawners = new List<GameObject>();

        if (!_chunkCoordonates.TryGetValue(chunk.transform.position, out ChunkToGenerate actualChunk)) return nearAvailableEnemySpawners;

        foreach (Vector3 neighbourChunkPos in _straightNeighboursChunkPos.Keys)
        {
            Vector3 nextChunkPos = chunk.transform.position + neighbourChunkPos;
            if (!HasAPathBetweenChunks(actualChunk, nextChunkPos, _straightNeighboursChunkPos[neighbourChunkPos])) continue;

            foreach (GameObject chunkGo in _generatedMap.Keys)
            {
                if (chunkGo.TryGetComponent(out ChunkData chunkData) && chunkGo.transform.position == nextChunkPos)
                {
                    nearAvailableEnemySpawners.AddRange(chunkData.EnemySpawners);
                    break;
                }
            }
        }

        return nearAvailableEnemySpawners;
    }

    private bool HasAPathBetweenChunks(ChunkToGenerate chunkToGenerate, Vector3 secondChunkPos, Direction direction)
    {

        if (_chunkCoordonates.TryGetValue(secondChunkPos, out ChunkToGenerate secondChunk))
        {
            bool firstHasDirection = chunkToGenerate.ChunkRotation.Directions.Contains(direction);
            bool secondHasDirection = false;

            switch (direction)
            {
                case Direction.Up:
                    secondHasDirection = secondChunk.ChunkRotation.IsUpRotation;
                    break;
                case Direction.Down:
                    secondHasDirection = secondChunk.ChunkRotation.IsDownRotation;
                    break;
                case Direction.Left:
                    secondHasDirection = secondChunk.ChunkRotation.IsLeftRotation;
                    break;
                case Direction.Right:
                    secondHasDirection = secondChunk.ChunkRotation.IsRightRotation;
                    break;
            }

            return firstHasDirection && secondHasDirection;
        }

        return false;
    }
}

public struct ChunkToGenerate
{
    public Chunk Chunk;
    public ChunkRotation ChunkRotation;

    public ChunkToGenerate(Chunk chunk, ChunkRotation chunkRotation)
    {
        Chunk = chunk;
        ChunkRotation = chunkRotation;
    }
}
