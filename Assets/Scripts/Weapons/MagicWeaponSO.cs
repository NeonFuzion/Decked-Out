using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon/MagicWeapon")]
public class MagicWeaponSO : WeaponSO
{
    [SerializeField] int projectileCount, projectileSpread;
    [SerializeField] GameObject prefabProjectile;

    public int ProjectileCount { get => projectileCount; }
    public int ProjectileSpread { get => projectileSpread; }
    public GameObject PrefabProjectile { get => prefabProjectile; }

    public override void AttackActionHandle(int attackIndex, Transform transform, Vector2 mousePosition, Shooter shooter)
    {
        int range = projectileSpread;
        Vector2 offset = Vector2.zero;

        for (int i = 0; i < projectileCount; i++)
        {
            if (projectileCount > 1)
                offset = new Vector2((Random.value * 2) - 1, (Random.value * 2) - 1) * range;
            if (prefabProjectile.GetComponent<Projectile>().MaxHeight == 0)
                offset = (mousePosition - (Vector2)transform.position).normalized * 100 + offset * 50;

            Projectile projectile;
            shooter.FireProjectile(prefabProjectile, mousePosition + offset, out projectile, FiringMode.FirePoint);
            projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) => OnHit(colliders, projectile, attackIndex));
        }
    }

    void OnHit(Collider2D[] colliders, Projectile projectile, int attackIndex)
    {
        colliders = colliders.Where(collider => collider.GetComponent<Enemy>() || collider.GetComponent<BreakableRoomObject>()).ToArray();

        if (colliders.Length == 0) return;
        AttackSequenceData attackStage = AttackComboData[attackIndex];
        EventManager.InvokeOnEnemyDataAcquired(colliders, new (Element, projectile.transform.position, attackStage.Damage, attackStage.Stagger));

        if (!projectile.gameObject) return;
        Destroy(projectile.gameObject);
    }

    public override void AttackAnimationHandle(int animationIndex, Transform transform, Animator animator)
    {
        animator.SetFloat("AttackSwingSpeed", AttackSpeed);
        animator.CrossFade(GetAnimationByIndex(animationIndex), 0, 0);
    }
}
