using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment/Accessory")]
public class Accessory : Equipment
{
    [SerializeField] SetBonus setBonus;
    [SerializeField] PassiveEffectSO passiveEffectSO;

    public SetBonus SetBonus { get => setBonus; }
    public PassiveEffectSO PassiveEffectSO { get => passiveEffectSO; }
}