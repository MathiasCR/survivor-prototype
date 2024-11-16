using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterObject", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Base Character Settings")]
    [SerializeField] public int MaxLife;
    [SerializeField] public int Luck;
    [SerializeField] public float Sway;
    [SerializeField] public float MagnetRadius;
    [SerializeField] public float DefaultFilterTimer;

    [Header("Character Speed Settings")]
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float ReloadSpeed;
    [SerializeField] public float AimingMoveSpeed;
    [SerializeField] public float SwitchWeaponSpeed;
    [SerializeField] public float MoveAnimationSpeed;
}
