using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] float jumpTime, jumpCD;

    float curJumpTime, curJumpCD;
    bool jumping;

    Vector2 knockback;
    BoxCollider2D bc;
    SpriteRenderer sr;
    Vector2 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        curJumpCD = 0;
        curJumpTime = 0;
        jumping = false;

        knockback = Vector2.zero;
        targetPos = Vector2.zero;
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        sr.enabled = false;
        GetComponent<Health>().Initialize(hp, def);
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;
        if (curJumpCD > 0)
        {
            curJumpCD -= Time.deltaTime;
            return;
        }
        if (jumping)
        {
            curJumpTime -= Time.deltaTime;
            if (curJumpTime < 0 || Vector3.Distance(target.position, transform.position) < 1.75f)
                animator.CrossFade("SlimeLanding", 0, 0);
            return;
        }
        animator.CrossFade("SlimeBounce", 0, 0);
        targetPos = target.position;
        Movement();
        bc.enabled = false;
        sr.enabled = true;
        curJumpTime = jumpTime;
        jumping = true;
    }

    private void FixedUpdate()
    {
        if (!jumping)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        if (Vector3.Distance(targetPos, transform.position) < 0.2f) return;
    }

    public void OnLanding()
    {
        sr.enabled = false;
        bc.enabled = true;
        jumping = false;
        curJumpCD = jumpCD;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, 1))
        {
            Player script = col.gameObject.GetComponent<Player>();

            if (!script) continue;
            col.GetComponent<Health>()?.TakeDamage(atk, Element.Water);
            break;
        }
    }
}
