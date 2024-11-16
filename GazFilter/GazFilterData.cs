using UnityEngine;

[CreateAssetMenu(fileName = "GazFilter", menuName = "ScriptableObjects/GazFilterObject", order = 8)]
public class GazFilterData : ScriptableObject
{
    [Header("Base Character Settings")]
    [SerializeField] public GazFilterType GazFilterType;
    [SerializeField] public Rarity GazFilterRarity;
    [SerializeField] public int FilterTimerBonus;
    [SerializeField] public Sprite GazFilterIcon;
}

public enum GazFilterType
{
    BasicGazFilter,
}
