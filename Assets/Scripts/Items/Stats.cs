using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Stats
{
    static Dictionary<PlayerStat, float> statBoosts = new Dictionary<PlayerStat, float>()
    {
        { PlayerStat.Attack, 0.1f },
        { PlayerStat.Defense, 0.1f },
        { PlayerStat.Health, 0.1f },
        { PlayerStat.CriticalChance, 0.2f },
        { PlayerStat.CriticalDamage, 0.4f },
        { PlayerStat.EnergyRecharge, 0.3f }
    };

    public static bool IsPercentage(PlayerStat stat)
    {
        return (int)stat >= 3;
    }
}

public enum PlayerStat
{
    Attack, Defense, Health,
    CriticalChance, CriticalDamage, EnergyRecharge,
    PhysicalDamageBonus, FireDamageBonus, WaterDamageBonus,
    WindDamageBonus, EarthDamageBonus, ElectricDamageBonus,
    NatureDamageBonus, IceDamageBonus
}
