using System;
using UnityEngine;

public abstract class SkillTome : Equipment
{
    [SerializeField] Sprite uiSprite;
    [SerializeField] CombatResource combatResource;
    [SerializeField] int resourceCost;
    [SerializeField] float cooldown;

    public CombatResource CombatResource { get => combatResource; }
    public int ResourceCost { get => resourceCost; }
    public float Cooldown { get => cooldown; }

    public abstract void ActivateEffects(Player player, int index);
}

public enum CombatResource { None, Mana, Soul, Adrenaline }