using System;
using UnityEngine;

public class Skill
{
    [SerializeField] CombatResource combatResource;
    [SerializeField] SkillHitData[] catalystHitData;

    public CombatResource CombatResource { get => combatResource; }
    public SkillHitData[] CatalystHitData { get => catalystHitData; }
}

[Serializable]
public class SkillHitData
{
    [SerializeField] float cooldown;
    [SerializeField] float resourceCost;
    [SerializeField] StatBoost[] multipliers;

    public float Cooldown { get => cooldown; }
    public float ResourceCost { get => resourceCost; }
    public StatBoost[] Multipliers { get => multipliers; }
}

public enum CombatResource { None, Mana, Soul, Adrenaline }