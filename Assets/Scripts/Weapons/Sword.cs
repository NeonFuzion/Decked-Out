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

    public override void AttackActionHandle(int damage, bool isCrit, Transform transform)
    {
        Vector3 hitPos = transform.right * attackRange * 0.5f;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position + hitPos, attackRange))
        {
            if (col.GetComponent<Player>()) continue;
            Health health = col.GetComponent<Health>();

            if (!health) continue;
            health.TakeDamage(damage, transform.parent.position, isCrit, Knockback);

            if (health.HP > 0) continue;
            EventManager.InvokeOnKill();
        }
    }

    public override void AttackAnimationHandle(int animationIndex, Transform transform)
    {
        Transform slash = transform.GetChild(1);
        Animator animator = slash.GetComponent<Animator>();

        slash.position = transform.position + transform.right * attackRange;
        slash.localScale = Vector3.one * attackRange * 1.2f;

        animator.SetFloat("AtkSpdMulti", AttackSpeed);
        animator.CrossFade(GetSlashAnimationByIndex(animationIndex), 0, 0);
    }
}
