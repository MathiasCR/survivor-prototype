using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _enemiesLayer;
    [SerializeField] private LayerMask _collectableLayer;
    [SerializeField] private LayerMask _obstructionLayer;
    [SerializeField] private LayerMask _interactableLayer;

    public static GameLayers Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask FloorLayer
    {
        get => _floorLayer;
    }

    public LayerMask EnemiesLayer
    {
        get => _enemiesLayer;
    }

    public LayerMask PlayerLayer
    {
        get => _playerLayer;
    }

    public LayerMask InteractableLayer
    {
        get => _interactableLayer;
    }

    public LayerMask ObstructionLayer
    {
        get => _obstructionLayer;
    }

    public LayerMask CollectableLayer
    {
        get => _collectableLayer;
    }
}
