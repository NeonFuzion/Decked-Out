using System.Collections;
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
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;

            Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, radius)
                .Where(c => c.GetComponent<Enemy>() != null)
                .ToArray();

            if (enemyColliders.Length == 0) continue;
            AttackData attackData = new(element, transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger, -(int)pullStrength);
            EventManager.InvokeOnEnemyDataAcquired(enemyColliders, attackData);
        }

        Destroy(gameObject);
    }
}
