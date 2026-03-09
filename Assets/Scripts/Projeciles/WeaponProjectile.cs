using UnityEngine;

[CreateAssetMenu]
public class WeaponProjectile : ProjectileEffect
{
    StatBoost[] multipliers;

    public StatBoost[] Multipliers { get => multipliers; }

    public void Initialize(StatBoost[] multipliers)
    {
        this.multipliers = multipliers;
    }

    public override void ActivateEffect(Vector2 position, Collider2D[] colliders)
    {
        
    }
}
