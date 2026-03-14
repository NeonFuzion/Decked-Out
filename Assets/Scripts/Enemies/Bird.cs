using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bird : Enemy
{
    [SerializeField] int chargeSpeed, minChargeDistance, chargeCooldown;
    [SerializeField] float chargeDuration, telegraphDuration;

    bool isResting, damageDealt;

    BirdState birdState;
    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        birdState = BirdState.Idle;
        isResting = false;
        damageDealt = true;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;

        switch (birdState)
        {
            case BirdState.Idle:
                animator.CrossFade("Flapping", 0, 0);
                transform.localScale = new Vector3(transform.position.x > target.position.x ? 1 : -1, 1);
                Movement();

                if (isResting) break;
                if (Vector2.Distance(transform.position, target.position) >= minChargeDistance) break;
                animator.CrossFade("Charging", 0, 0);
                StartCoroutine(ChargeCoroutine());
                break;
            case BirdState.Charging:
                if (damageDealt) return;
                Collider2D collision = Physics2D.OverlapCircle(transform.position, 1);
                
                if (collision.transform != target) break;
                damageDealt = true;
                collision.GetComponent<Health>().TakeDamage(atk, Element.Wind);
                break;
        }
    }

    IEnumerator ChargeCoroutine()
    {
        birdState = BirdState.Telegraphing;
        rb.linearVelocity = Vector2.zero;
        Vector2 fallBackTarget = target.position;
        yield return new WaitForSeconds(telegraphDuration);
        rb.linearVelocity = ((target ? target.position : fallBackTarget) - transform.position).normalized * chargeSpeed;

        birdState = BirdState.Charging;
        damageDealt = false;
        boxCollider.excludeLayers = wallLayer;
        yield return new WaitForSeconds(chargeDuration);
        birdState = BirdState.Idle;
        boxCollider.excludeLayers = new ();

        isResting = true;
        yield return new WaitForSeconds(chargeCooldown);
        isResting = false;
    }
}

enum BirdState { None, Idle, Charging, Telegraphing };
