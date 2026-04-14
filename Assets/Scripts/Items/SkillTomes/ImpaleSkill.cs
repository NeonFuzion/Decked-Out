using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/Impale")]
public class ImpaleSkill : SkillTome
{
    [SerializeField] ProjectileData icicleProjectile;

    public override void ActivateEffects(Player player, int index)
    {
        Shooter shooter = player.GetComponentInChildren<Shooter>();
        Projectile projectile;
        shooter.FireProjectile(icicleProjectile, MainCamera.MouseWorldPosition(), out projectile);
    }
}
