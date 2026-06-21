using System.Collections;
using UnityEngine;

public class Bird : Enemy
{
    [SerializeField] int chargeSpeed, minChargeDistance, chargeCooldown;
    [SerializeField] float chargeDuration, telegraphDuration;

    bool isResting, damageDealt;
    int chargingAnim = Animator.StringToHash("Charging");

    BirdState birdState;

    protected override int IdleAnim => Animator.StringToHash("Flapping");

    new void Start()
    {
        base.Start();

        birdState = BirdState.Idle;
        isResting = false;
        damageDealt = true;
    }

    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;

        switch (birdState)
        {
            case BirdState.Idle:
                if (IsStaggered) break;
                transform.localScale = new Vector3(transform.position.x > target.position.x ? 1 : -1, 1);
                Movement();

                if (isResting) break;
                if (Vector2.Distance(transform.position, target.position) >= minChargeDistance) break;
                animator.CrossFade(chargingAnim, 0, 0);
                StartCoroutine(ChargeCoroutine());
                break;
            case BirdState.Charging:
                if (damageDealt) return;
                Collider2D collision = Physics2D.OverlapCircle(transform.position, 1);

                if (collision.transform != target) break;
                damageDealt = true;
                collision.GetComponent<Health>().TakeDamage(attack, Element.Wind, transform.position);
                break;
        }
    }

    IEnumerator ChargeCoroutine()
    {
        birdState = BirdState.Telegraphing;
        rigidbody.linearVelocity = Vector2.zero;
        Vector2 fallBackTarget = target.position;
        yield return new WaitForSeconds(telegraphDuration);
        rigidbody.linearVelocity = ((target ? target.position : fallBackTarget) - transform.position).normalized * chargeSpeed;

        birdState = BirdState.Charging;
        damageDealt = false;
        yield return new WaitForSeconds(chargeDuration);
        birdState = BirdState.Idle;
        rigidbody.linearVelocity = Vector2.zero;
        animator.CrossFade(IdleAnim, 0, 0);

        isResting = true;
        yield return new WaitForSeconds(chargeCooldown);
        isResting = false;
    }

    public override void OnStagger()
    {
        base.OnStagger();
        isResting = false;
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        birdState = BirdState.Idle;
    }
}

enum BirdState { None, Idle, Charging, Telegraphing }
