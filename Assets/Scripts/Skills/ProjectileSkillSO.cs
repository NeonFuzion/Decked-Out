using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/Projectile")]
public class ProjectileSkillSO : SkillTomeSO
{
    [SerializeField] GameObject prefabProjectile;

    public override void ActivateEffects(SkillManager skillManager, int index)
    {
        Shooter shooter = skillManager.GetComponentInChildren<Shooter>();
        Projectile projectile;
        shooter.FireProjectile(prefabProjectile, MainCamera.MouseWorldPosition(), out projectile, FiringMode.Radial);
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