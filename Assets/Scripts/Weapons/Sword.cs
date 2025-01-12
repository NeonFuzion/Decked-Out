using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon/Sword")]
public class Sword : Weapon
{
    [SerializeField] float attackRange;
    [SerializeField] List<string> slashAnimations;

    public float AttackRange { get => attackRange; }
    public List<string> SwingAnimations { get => slashAnimations; }

    public string GetSlashAnimationByIndex(int index)
    {
        return slashAnimations[index];
    }
}
