using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    [SerializeField] float knockbackResistance = 0, movementSpeed = 5;
    [SerializeField] UnityEvent onKnockbackStarted, onKnockbackEnded;
    
    bool isActive;

    Rigidbody2D rigidbody;
    Vector2 movementInput;

    void Awake()
    {
        isActive = true;

        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!isActive) return;
        rigidbody.linearVelocity = movementInput;
    }

    IEnumerator KnockbackCoroutine(Vector2 incomingAttack, float knockback)
    {
        onKnockbackStarted?.Invoke();
        SetImmobile();
        rigidbody.AddForce(((Vector2)transform.position - incomingAttack).normalized * knockback * (1 - knockbackResistance), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        SetMobile();
        SetMovement(Vector2.zero);
        onKnockbackEnded?.Invoke();
    }

    public void ApplyKnockback(Vector2 origin, float knockback)
    {
        if (knockbackResistance >= 1) return;
        StopAllCoroutines();
        StartCoroutine(KnockbackCoroutine(origin, knockback));
    }

    public void SetMobile()
    {
        isActive = true;
    }

    public void SetImmobile()
    {
        isActive = false;
        rigidbody.linearVelocity = Vector2.zero;
    }

    public void SetMovementDirection(Vector2 direction)
    {
        movementInput = direction.normalized * movementSpeed;
    }

    public void SetMovement(Vector2 movement)
    {
        movementInput = movement;
    }
}
