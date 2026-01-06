using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public override void AttackActionHandle(int attackIndex, Transform transform, Vector2 mousePosition)
    {
        Vector3 hitPos = transform.right * (attackRange + 1);
        EventManager.InvokeOnEnemyDataAcquired(Physics2D.OverlapCircleAll(transform.position + hitPos, attackRange), new (Element, transform.position, GetMultipliersByIndex(attackIndex)));
    }

    public override void AttackAnimationHandle(int animationIndex, Transform weaponParent)
    {
        // sword animations
        Transform weaponHandle = weaponParent.GetChild(1);
        Animator animator = weaponHandle.GetComponent<Animator>();

        animator.SetFloat("AttackSwingSpeed", AttackSpeed);
        animator.CrossFade(GetAnimationByIndex(animationIndex), 0, 0);

        // slash animations
        Transform slash = weaponParent.GetChild(0);
        Animator slashAnimator = slash.GetComponentInChildren<Animator>();

        slash.position = weaponParent.position + weaponParent.right * (attackRange + 1);
        slash.localScale = Vector3.one * (attackRange + 1);

        slashAnimator.SetFloat("AtkSpdMulti", AttackSpeed);
        slashAnimator.CrossFade(GetSlashAnimationByIndex(animationIndex), 0, 0);
    }
}
