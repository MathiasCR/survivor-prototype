using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/BiomeScriptableObject", order = 9)]
public class Biome : ScriptableObject
{
    [SerializeField] public bool Unlocked;
    [SerializeField] public Sprite BiomeIcon;
    [SerializeField] public BiomeType BiomeType;
}

public enum BiomeType
{
    Street,
}
