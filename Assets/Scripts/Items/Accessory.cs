using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Accessory")]
public class Accessory : Equipment
{
    [SerializeField] RelicSlot relicSlot;
    [SerializeField] PlayerStat mainStat;
    [SerializeField] SetBonus setBonus;
    [SerializeField] float statBoost;

    public RelicSlot RelicSlot { get => relicSlot; }
    public PlayerStat MainStat { get => mainStat; }
    public SetBonus SetBonus { get => setBonus; }
    public float StatBoost {  get => statBoost; }
}

public enum RelicSlot { Headpiece, Chestpiece, Timepiece, Leggings, Flower }
