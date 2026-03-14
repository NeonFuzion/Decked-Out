using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon/MagicWeapon")]
public class MagicWeapon : Weapon
{
    [SerializeField] int projectileCount, projectileSpread;
    [SerializeField] GameObject prefabProjectile;
    [SerializeField] ProjectileData projectileData;

    public int ProjectileCount { get => projectileCount; }
    public int ProjectileSpread { get => projectileSpread; }
    public GameObject PrefabProjectile { get => prefabProjectile; }
    public ProjectileData ProjectileData { get => projectileData; }

    public override void AttackActionHandle(int attackIndex, Transform transform, Vector2 mousePosition, Shooter shooter)
    {
        int range = projectileSpread;
        Vector2 offset = Vector2.zero;

        for (int i = 0; i < projectileCount; i++)
        {
            if (projectileCount > 1)
                offset = new Vector2((Random.value * 2) - 1, (Random.value * 2) - 1) * range;
            if (projectileData.MaxHeight == 0)
                offset = (mousePosition - (Vector2)transform.position).normalized * 100 + offset * 50;

            Projectile projectile;
            shooter.FireProjectile(projectileData, mousePosition + offset, out projectile);
            WeaponProjectile weaponProjectile = projectile.ProjectileEffect as WeaponProjectile;
            weaponProjectile.Initialize(GetMultipliersByIndex(attackIndex));
            projectile.OnHit.AddListener(OnHit);
        }
    }

    void OnHit(Collider2D[] colliders, Projectile projectile)
    {
        colliders = colliders.Where(collider => collider.GetComponent<Enemy>() || collider.GetComponent<BreakableRoomObject>()).ToArray();

        if (colliders.Length == 0) return;
        WeaponProjectile weaponProjectile = projectile.ProjectileEffect as WeaponProjectile;
        EventManager.InvokeOnEnemyDataAcquired(colliders, new (Element, projectile.transform.position, weaponProjectile.Multipliers));

        if (!projectile.gameObject) return;
        Destroy(projectile.gameObject);
    }

    public override void AttackAnimationHandle(int animationIndex, Transform transform, Animator animator)
    {
        animator.SetFloat("AttackSwingSpeed", AttackSpeed);
        animator.CrossFade(GetAnimationByIndex(animationIndex), 0, 0);
    }
}
