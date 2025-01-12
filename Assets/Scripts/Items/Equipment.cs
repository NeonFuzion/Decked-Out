using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    [SerializeField] EquipmentEffect equipmentEffect;

    public EquipmentEffect EquipmentEffect { get => equipmentEffect; }
}
