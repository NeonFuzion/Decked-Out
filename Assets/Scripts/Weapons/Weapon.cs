using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MainHand
{
    [SerializeField] int attack;
    [SerializeField] float attackSpeed, knockback;
    [SerializeField] StatBoost substat;
    [SerializeField] WeaponHoldStyle weaponHoldStyle;
    [SerializeField] Element element;
    [SerializeField] AttackSequenceData[] attackComboData;

    public float AttackSpeed { get => attackSpeed; }
    public float Knockback { get => knockback; }
    
    public int Attack { get => attack; }
    public StatBoost Substat { get => substat; }
    public WeaponHoldStyle WeaponHoldStyle { get => weaponHoldStyle; }
    public Element Element { get => element; }
    public AttackSequenceData[] Animations { get => attackComboData; }

    public int GetNextAnimationIndex(int index)
    {
        return attackComboData.Length > 1 ? (index + 1) % attackComboData.Length : 0;
    }

    public AttackSequenceData GetAttackSequenceDataByIndex(int index)
    {
        return attackComboData[Mathf.Clamp(index, 0, attackComboData.Length - 1)];
    }

    public string GetAnimationByIndex(int index)
    {
        return GetAttackSequenceDataByIndex(index).Animation;
    }

    public StatBoost[] GetMultipliersByIndex(int index)
    {
        return GetAttackSequenceDataByIndex(index).Multipliers;
    }

    public abstract void AttackActionHandle(int attackIndex, Transform transform, Vector2 mousePosition, Shooter shooter);
    
    public abstract void AttackAnimationHandle(int animationIndex, Transform transform, Animator animator);
}

public enum WeaponHoldStyle { Static, Mouse }
public enum Element { Physical, Fire, Water, Wind, Earth, Electric, Nature, Ice }

[Serializable]
public class AttackSequenceData
{
    [SerializeField] string animation;
    [SerializeField] StatBoost[] multipliers;

    public string Animation { get => animation; }
    public StatBoost[] Multipliers { get => multipliers; }
}