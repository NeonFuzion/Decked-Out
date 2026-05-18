using System;
using UnityEngine;

public abstract class SkillTomeSO : EquipmentSO
{
    [SerializeField] CombatResource combatResource;
    [SerializeField] Element element;
    [SerializeField] int resourceCost;
    [SerializeField] float cooldown;
    [SerializeField] DamageStaggerPair[] damageStaggerPairs;

    public CombatResource CombatResource { get => combatResource; }
    public Element Element { get => element; }
    public int ResourceCost { get => resourceCost; }
    public float Cooldown { get => cooldown; }
    public DamageStaggerPair[] DamageStaggerPairs { get => damageStaggerPairs; }

    public abstract void ActivateEffects(Player player, int index);
}

public enum CombatResource { None, Mana, Soul, Adrenaline }