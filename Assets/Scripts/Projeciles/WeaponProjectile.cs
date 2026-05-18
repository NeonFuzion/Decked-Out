using UnityEngine;

public class WeaponProjectile : ProjectileEffect
{
    int damage, stagger;

    public int Damage { get => damage; }
    public int Stagger { get => stagger; }

    public void Initialize(int damage, int stagger)
    {
        this.damage = damage;
        this.stagger = stagger;
    }

    public override void ActivateEffect(Vector2 position, Collider2D[] colliders)
    {
        
    }
}
