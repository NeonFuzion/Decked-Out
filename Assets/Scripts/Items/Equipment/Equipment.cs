using UnityEngine;

[System.Serializable]
public class Equipment
{
    [SerializeField] EquipmentSO equipmentData;

    public EquipmentSO EquipmentData => equipmentData;

    public Equipment(EquipmentSO equipmentData)
    {
        this.equipmentData = equipmentData;
    }
}
