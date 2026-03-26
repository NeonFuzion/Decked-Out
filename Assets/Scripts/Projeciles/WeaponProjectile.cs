using UnityEngine;

[CreateAssetMenu]
public class WeaponProjectile : ProjectileEffect
{
    int damage;

    public int Damage { get => damage; }

    public void Initialize(int damage)
    {
        this.damage = damage;
    }

    public override void ActivateEffect(Vector2 position, Collider2D[] colliders)
    {
        
    }
}
