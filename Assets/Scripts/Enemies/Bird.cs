using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bird : Enemy
{
    [SerializeField] int chargeSpeed, minChargeDistance, chargeCooldown;
    [SerializeField] float chargeDuration, telegraphDuration;

    BirdState birdState;
    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        birdState = BirdState.Idle;

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        GetComponent<Health>().Initialize(hp, def);
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;
        if (birdState == BirdState.Charging || birdState == BirdState.Telegraphing) return;
        if (Vector2.Distance(transform.position, target.position) < minChargeDistance)
        {
            if (birdState != BirdState.Resting)
            {
                animator.CrossFade("Charging", 0, 0);
                StartCoroutine(ChargeCoroutine());
            }
        }
        else
        {
            animator.CrossFade("Flapping", 0, 0);
            transform.localScale = new Vector3(transform.position.x > target.position.x ? 1 : -1, 1);
            Movement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (birdState != BirdState.Charging) return;
        if (collision.transform == target) collision.GetComponent<Health>().TakeDamage(atk);

        Rigidbody2D rigidbody = collision.GetComponent<Rigidbody2D>();
        if (!rigidbody) return;
        if (rigidbody.bodyType != RigidbodyType2D.Static) return;
        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator ChargeCoroutine()
    {
        birdState = BirdState.Telegraphing;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(telegraphDuration);
        rb.linearVelocity = (target.position - transform.position).normalized * chargeSpeed;

        birdState = BirdState.Charging;
        boxCollider.isTrigger = true;
        yield return new WaitForSeconds(chargeDuration);
        boxCollider.isTrigger = false;
        birdState = BirdState.Resting;

        yield return new WaitForSeconds(chargeCooldown);
        birdState = BirdState.Idle;
    }
}

enum BirdState { Idle, Charging, Resting, Telegraphing };
