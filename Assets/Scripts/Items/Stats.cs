using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Stats
{
    public static bool IsPercentage(PlayerStat stat)
    {
        return stat >= PlayerStat.StaggerMultiplier;
    }
}

public enum PlayerStat
{
    None,
    Attack, Magic, Defense, Health,
    ReactionAffinity, StaggerMultiplier, DefensePenetration,
    PhysicalDamageBonus, FireDamageBonus, WaterDamageBonus,
    WindDamageBonus, EarthDamageBonus, LightningDamageBonus,
    NatureDamageBonus, IceDamageBonus
}

[System.Serializable]
public class StatBoost
{
    [SerializeField] PlayerStat stat;
    [SerializeField] float amount;

    public StatBoost(PlayerStat stat, float amount)
    {
        this.stat = stat;
        this.amount = amount;
    }

    public void ChangeAmount(float amount) => this.amount += amount;
    public void SetAmount(float amount) => this.amount = amount;
    
    public PlayerStat Stat { get => stat; }
    public float Amount { get => amount; }
    public override string ToString()
    {
        string amountStr = Stats.IsPercentage(stat) ? amount * 100 + "%" : amount + "";
        return $"{stat} +{amountStr}";
    }
}