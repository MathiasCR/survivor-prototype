using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/EquipmentScriptableObject", order = 3)]
public class Equipment : ScriptableObject
{
    [SerializeField] public bool CanHaveTop;
    [SerializeField] public bool CanHaveEyes;
    [SerializeField] public bool CanHaveEars;
    [SerializeField] public bool CanHaveFace;
    [SerializeField] public bool CanHaveBody;
    [SerializeField] public Sprite EquipmentIcon;
    [SerializeField] public EquipmentType EquipmentType;

    public SkinnedMeshRenderer GetSkinnedMeshOfEquipment(SkinnedMeshRenderer[] renderers)
    {
        return Array.Find(renderers, (mesh) => mesh.gameObject.name == name);
    }

    public bool DoOthersAllowThis(List<Equipment> equipments)
    {
        bool canEquip = true;

        foreach (Equipment equipment in equipments)
        {
            switch (EquipmentType)
            {
                case EquipmentType.Top:
                    if (!equipment.CanHaveTop) canEquip = false;
                    break;
                case EquipmentType.Eyes:
                    if (!equipment.CanHaveEyes) canEquip = false;
                    break;
                case EquipmentType.Ears:
                    if (!equipment.CanHaveEars) canEquip = false;
                    break;
                case EquipmentType.Face:
                    if (!equipment.CanHaveFace) canEquip = false;
                    break;
                case EquipmentType.Body:
                    if (!equipment.CanHaveBody) canEquip = false;
                    break;
            }

            if (!canEquip)
            {
                return canEquip;
            }
        }

        return canEquip;
    }

    public bool hasRestriction()
    {
        return !CanHaveTop || !CanHaveEars || !CanHaveEyes || !CanHaveFace || !CanHaveBody;
    }



    /*public void SwitchPlayerEquipment(Equipment equipment, Equipment previousEquipment)
    {
        SkinnedMeshRenderer previousMesh = previousEquipment.GetSkinnedMeshOfEquipment(GetComponentsInChildren<SkinnedMeshRenderer>());
        SkinnedMeshRenderer currentMesh = equipment.GetSkinnedMeshOfEquipment(GetComponentsInChildren<SkinnedMeshRenderer>());

        if (previousMesh != null)
        {
            previousMesh.enabled = false;
            EquippedItems.Remove(previousEquipment);
        }

        if (currentMesh != null)
        {
            currentMesh.enabled = true;
            EquippedItems.Add(equipment);
        }
    }

    public void EquipPlayer(Equipment equipment)
    {
        equipment.GetSkinnedMeshOfEquipment(GetComponentsInChildren<SkinnedMeshRenderer>()).enabled = true;
        EquippedItems.Add(equipment);
    }

    public void UnEquipPlayer(Equipment equipment)
    {
        equipment.GetSkinnedMeshOfEquipment(GetComponentsInChildren<SkinnedMeshRenderer>()).enabled = false;
        EquippedItems.Remove(equipment);
    }*/
}

public enum EquipmentType
{
    Top,
    Eyes,
    Ears,
    Face,
    Body,
    None
}