using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    [SerializeField] float knockbackResistance = 0, movementSpeed = 5, accelerationRate = 100, decelerationRate = 100;
    [SerializeField] UnityEvent onKnockbackStarted, onKnockbackEnded;
    
    bool isActive;

    Rigidbody2D rigidbody;
    Vector2 movementInput, currentVelocity;

    public float MovementSpeed => movementSpeed;
    public float AccelerationRate => accelerationRate;
    public float DecelerationRate => decelerationRate;

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
        float xAcceleration = (movementInput.x == 0 ? decelerationRate : accelerationRate) * Time.fixedDeltaTime;
        float yAcceleration = (movementInput.y == 0 ? decelerationRate : accelerationRate) * Time.fixedDeltaTime;

        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, movementInput.x, xAcceleration);
        currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, movementInput.y, yAcceleration);

        rigidbody.linearVelocity = currentVelocity;
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
        if (!gameObject.activeInHierarchy) return;
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

    public void SetSpeed(float movementSpeed)
    {
        this.movementSpeed = Mathf.Max(movementSpeed, 0);
    }

    public void IncrementSpeed(float movementSpeedDelta)
    {
        movementSpeed += Mathf.Max(movementSpeedDelta, -movementSpeed);
    }

    public void SetAcceleration(float accelerationRate)
    {
        this.accelerationRate = Mathf.Max(accelerationRate, 0);
    }

    public void IncrementAcceleration(float accelerationRateDelta)
    {
        accelerationRate += Mathf.Max(accelerationRateDelta, -accelerationRate);
    }

    public void SetDeceleration(float decelerationRate)
    {
        this.decelerationRate = Mathf.Max(decelerationRate, 0);
    }

    public void IncrementDeceleration(float decelerationRateDelta)
    {
        decelerationRate += Mathf.Max(decelerationRateDelta, -decelerationRate);
    }
}
