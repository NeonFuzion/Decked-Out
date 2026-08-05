using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackEffect : MonoBehaviour
{
    [SerializeField] float knockbackResistance = 0;
    [SerializeField] UnityEvent onKnockbackStarted, onKnockbackEnded;

    new Rigidbody2D rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyKnockback(Vector2 origin, float knockback)
    {
        StopAllCoroutines();
        StartCoroutine(KnockbackCoroutine(origin, knockback));
    }

    IEnumerator KnockbackCoroutine(Vector2 incomingAttack, float knockback)
    {
        onKnockbackStarted?.Invoke();
        rigidbody.AddForce(((Vector2)transform.position - incomingAttack).normalized * knockback * (1 - knockbackResistance), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        onKnockbackEnded?.Invoke();
    }
}
