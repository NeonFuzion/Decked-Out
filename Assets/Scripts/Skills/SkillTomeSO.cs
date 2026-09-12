using System;
using UnityEngine;

public abstract class SkillTomeSO : EquipmentSO
{
    [SerializeField] int resourceCost;
    [SerializeField] float cooldown;
    [SerializeField] CombatResource combatResource;
    [SerializeField] Element element;
    [SerializeField] GameObject skillObjectPrefab;
    [SerializeField] AttackBaseData[] damageStaggerPairs;

    public int ResourceCost { get => resourceCost; }
    public float Cooldown { get => cooldown; }
    public CombatResource CombatResource { get => combatResource; }
    public Element Element { get => element; }
    public GameObject SkillObjectPrefab => skillObjectPrefab;
    public AttackBaseData[] DamageStaggerPairs { get => damageStaggerPairs; }
}

public enum CombatResource { None, Mana, Soul, Adrenaline }

[Serializable]
public struct AttackBaseData
{
    [SerializeField] int damage, stagger;
    [SerializeField] float knockback;
    [SerializeField] DebuffData debuffData;

    public int Damage { get => damage; }
    public int Stagger { get => stagger; }
    public float Knockback => knockback;
    public DebuffData DebuffData => debuffData;
}