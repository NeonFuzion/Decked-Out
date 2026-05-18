using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/ProjectileTome")]
public class ProjectileSkillSO : SkillTomeSO
{
    [SerializeField] ProjectileSO projectileSO;

    public override void ActivateEffects(Player player, int index)
    {
        Shooter shooter = player.GetComponentInChildren<Shooter>();
        Projectile projectile;
        shooter.FireProjectile(projectileSO, MainCamera.MouseWorldPosition(), out projectile, FiringMode.Radial);
        projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) =>
        {
            DamageStaggerPair damageStaggerPair = DamageStaggerPairs[0];
            AttackData attackData = new (Element.Ice, projectile.transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger);
            EventManager.InvokeOnEnemyDataAcquired(colliders, attackData);
        });
    }
}

[Serializable]
public class DamageStaggerPair
{
    [SerializeField] int damage, stagger;

    public int Damage { get => damage; }
    public int Stagger { get => stagger; }
}