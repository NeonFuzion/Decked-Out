using System.Linq;
using UnityEngine;

public class DamageHolder : MonoBehaviour
{
    int damage, knockback, stagger;
    Element element;
    DebuffData debuffData;

    public int Damage => damage;
    public int Knockback => knockback;
    public int Stagger => stagger;
    public Element Element => element;
    DebuffData DebuffData => debuffData;

    void SendDamageData(Collider2D[] colliders, Vector2 position)
    {
        AttackData attackData = new (element, position, damage, stagger, knockback, debuffData);
        EventManager.OnEnemyDataAcquired.Invoke(colliders, attackData);
    }

    public void Initialize(int damage, int stagger, int knockback, Element element, DebuffData debuffData)
    {
        this.damage = damage;
        this.stagger = stagger;
        this.knockback = knockback;
        this.element = element;
        this.debuffData = debuffData;
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
