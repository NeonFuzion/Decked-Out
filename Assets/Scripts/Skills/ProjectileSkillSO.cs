using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/Projectile")]
public class ProjectileSkillSO : SkillTomeSO
{
    [SerializeField] GameObject prefabProjectile;

    public override void ActivateEffects(HotbarManager hotbarManage, int index)
    {
        Projectile projectile;
        hotbarManage.Shooter.FireProjectile(prefabProjectile, MainCamera.MouseWorldPosition(), out projectile, FiringMode.Radial);
        projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) =>
        {
            AttackBaseData damageStaggerPair = DamageStaggerPairs[0];
            AttackData attackData = new (Element.Ice, projectile.transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger, 1, damageStaggerPair.IsDebuffing ? DebuffData : new ());
            EventManager.OnEnemyDataAcquired.Invoke(colliders, attackData);
        });
    }
}