using UnityEngine;

public abstract class WeaponStat : MonoBehaviour
{
    public abstract void UpdateStat(WeaponData weaponData, bool isBonus);
}
