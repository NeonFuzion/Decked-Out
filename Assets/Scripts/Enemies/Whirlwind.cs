using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Whirlwind : MonoBehaviour
{
    DamageStaggerPair damageStaggerPair;
    float radius, duration, tickRate, pullStrength;
    Element element;

    public void Initialize(DamageStaggerPair pair, float radius, float duration, float tickRate, float pullStrength, Element element)
    {
        damageStaggerPair = pair;
        this.radius = radius;
        this.duration = duration;
        this.tickRate = tickRate;
        this.pullStrength = pullStrength;
        this.element = element;
        StartCoroutine(TornadoRoutine());
    }

    IEnumerator TornadoRoutine()
    {
        float elapsed = 0f;
        List<Rigidbody2D> enemyRigidbodies = new ();
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;

            Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, radius)
                .Where(c => c.GetComponent<Enemy>() != null)
                .ToArray();
            
            enemyRigidbodies = enemyColliders.Select(collider => collider.GetComponent<Rigidbody2D>()).ToList();
            enemyRigidbodies.ForEach(rigidbody =>
            {
                Vector2 pullDir = ((Vector2)transform.position - (Vector2)rigidbody.transform.position).normalized;
                rigidbody.linearVelocity += pullDir * pullStrength;
            });

            if (enemyColliders.Length == 0) continue;
            AttackData attackData = new(element, transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger);
            EventManager.InvokeOnEnemyDataAcquired(enemyColliders, attackData);
        }

        enemyRigidbodies.ForEach(rigidbody =>
        {
            rigidbody.linearVelocity = Vector2.zero;
        });
        Destroy(gameObject);
    }
}
