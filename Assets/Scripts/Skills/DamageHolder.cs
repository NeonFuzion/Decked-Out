using System.Linq;
using UnityEngine;

public class DamageHolder : MonoBehaviour
{
    int damage, knockback, stagger;
    Element element;

    public int Damage => damage;
    public int Knockback => knockback;
    public int Stagger => stagger;
    public Element Element => element;

    void SendDamageData(Collider2D[] colliders, Vector2 position)
    {
        AttackData attackData = new (element, position, damage, stagger, knockback);
        EventManager.InvokeOnEnemyDataAcquired(colliders, attackData);
    }

    public void Initialize(int damage, int stagger, int knockback)
    {
        this.damage = damage;
        this.stagger = stagger;
        this.knockback = knockback;
    }

    public void DealProjectileDamage(Collider2D[] colliders, Projectile projectile)
    {
        SendDamageData(colliders, projectile.transform.position);
    }

    public void DealDamage(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius).Where(collider => collider.gameObject != gameObject).ToArray();
        SendDamageData(colliders, transform.position);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
