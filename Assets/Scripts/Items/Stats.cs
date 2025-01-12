using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Stats
{
    static Dictionary<PieceType, PlayerStat[]> pieceMainStats = new Dictionary<PieceType, PlayerStat[]>()
    {
        { PieceType.Headpiece, new PlayerStat[] { PlayerStat.CriticalChance, PlayerStat.CriticalDamage} },
        { PieceType.Hands, new PlayerStat[] { PlayerStat.Attack, PlayerStat.EnergyRecharge } },
        { PieceType.Chestpiece, new PlayerStat[] { PlayerStat.Defense, PlayerStat.Health } },
        { PieceType.Leggings, new PlayerStat[] { PlayerStat.Attack, PlayerStat.Defense, PlayerStat.Health, PlayerStat.CriticalChance, PlayerStat.CriticalDamage, PlayerStat.EnergyRecharge } }
    };

    static Dictionary<PlayerStat, float> statBoosts = new Dictionary<PlayerStat, float>()
    {
        { PlayerStat.Attack, 0.1f },
        { PlayerStat.Defense, 0.1f },
        { PlayerStat.Health, 0.1f },
        { PlayerStat.CriticalChance, 0.2f },
        { PlayerStat.CriticalDamage, 0.4f },
        { PlayerStat.EnergyRecharge, 0.3f }
    };

    public static PlayerStat[] GetMainStats(PieceType pieceType)
    {
        return pieceMainStats[pieceType];
    }

    public static bool IsPercentage(PlayerStat stat)
    {
        if (stat == PlayerStat.CriticalChance) return true;
        else if (stat == PlayerStat.CriticalDamage) return true;
        else if (stat == PlayerStat.EnergyRecharge) return true;
        return false;
    }
}

public enum PieceType { Headpiece, Chestpiece, Leggings, Hands }
public enum PlayerStat { Attack, Defense, Health, CriticalChance, CriticalDamage, EnergyRecharge }
