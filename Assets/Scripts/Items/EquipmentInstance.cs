using UnityEngine;

[System.Serializable]
public class EquipmentInstance
{
    [SerializeField] EquipmentSO equipmentData;

    public EquipmentSO EquipmentData => equipmentData;
    public ItemSO ItemData => equipmentData;

    public EquipmentInstance(EquipmentSO equipmentData)
    {
        this.equipmentData = equipmentData;
    }
}
