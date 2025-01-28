using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : Equipment
{
    [SerializeField] float attack, attackSpeed, knockback;
    [SerializeField] WeaponHoldStyle weaponHoldStyle;
    [SerializeField] List<string> animations;

    public float Attack { get => attack; }
    public float AttackSpeed { get => attackSpeed; }
    public float Knockback { get => knockback; }
    public WeaponHoldStyle WeaponHoldStyle { get => weaponHoldStyle; }
    public List<string> Animations { get => animations; }

    public int GetNextAnimationIndex(int index)
    {
        return animations.Count > 1 ? (index + 1) % animations.Count : 0;
    }

    public string GetAnimationByIndex(int index)
    {
        return animations[index >= animations.Count ? animations.Count - 1 : index];
    }
}

public enum WeaponHoldStyle { Static, Mouse }
