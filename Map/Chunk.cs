using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chunk", menuName = "ScriptableObjects/ChunkObject", order = 6)]
public class Chunk : ScriptableObject
{
    [Header("Chunk Prefab")]
    [SerializeField] public GameObject ChunkPrefab;

    [Header("Chunk Direction Settings")]
    [SerializeField] public List<ChunkRotation> ChunkRotations;
}

[System.Serializable]
public struct ChunkRotation
{
    public Vector3 Rotation;
    public bool IsLeftRotation;
    public bool IsRightRotation;
    public bool IsUpRotation;
    public bool IsDownRotation;
    public List<Direction> Directions;
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    LeftUp,
    RightUp,
    LeftDown,
    RightDown
}