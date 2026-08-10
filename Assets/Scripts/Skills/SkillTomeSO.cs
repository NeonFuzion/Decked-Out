using System;
using UnityEngine;

public abstract class SkillTomeSO : EquipmentSO
{
    [SerializeField] int resourceCost;
    [SerializeField] float cooldown;
    [SerializeField] CombatResource combatResource;
    [SerializeField] Element element;
    [SerializeField] GameObject prefabParticleSystem;
    [SerializeField] DamageStaggerPair[] damageStaggerPairs;

    public int ResourceCost { get => resourceCost; }
    public float Cooldown { get => cooldown; }
    public CombatResource CombatResource { get => combatResource; }
    public Element Element { get => element; }
    public GameObject PrefabParticleSystem => prefabParticleSystem;
    public DamageStaggerPair[] DamageStaggerPairs { get => damageStaggerPairs; }

    public abstract void ActivateEffects(HotbarManager hotbarManager, int index);
}

public enum CombatResource { None, Mana, Soul, Adrenaline }