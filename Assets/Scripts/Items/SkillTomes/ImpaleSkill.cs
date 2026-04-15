using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/ImpaleTome")]
public class ImpaleSkill : SkillTome
{
    [SerializeField] ProjectileData icicleProjectile;

    public override void ActivateEffects(Player player, int index)
    {
        Shooter shooter = player.GetComponentInChildren<Shooter>();
        Projectile projectile;
        shooter.FireProjectile(icicleProjectile, MainCamera.MouseWorldPosition(), out projectile);
        projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) =>
        {
            AttackData attackData = new (Element.Ice, projectile.transform.position, DamageValues[0]);
            EventManager.InvokeOnEnemyDataAcquired(colliders, attackData);
        });
    }
}
