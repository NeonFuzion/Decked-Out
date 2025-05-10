using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment/Accessory")]
public class Accessory : Equipment
{
    [SerializeField] PlayerStat mainStat;
    [SerializeField] SetBonus setBonus;
    [SerializeField] StatBoost[] statBoosts;

    public PlayerStat MainStat { get => mainStat; }
    public SetBonus SetBonus { get => setBonus; }
    public StatBoost[] StatBoost { get => statBoosts; }
}

[System.Serializable]
public class StatBoost
{
    [SerializeField] PlayerStat stat;
    [SerializeField] float amount;
    
    public PlayerStat Stat { get => stat; }
    public float Amount { get => amount; }
}