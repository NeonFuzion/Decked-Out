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
    new void Start()
    {
        base.Start();

        curJumpCD = 0;
        curJumpTime = 0;
        jumping = false;

        knockback = Vector2.zero;
        targetPos = Vector2.zero;
        
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        sr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);

        if (curJumpCD > 0)
        {
            curJumpCD -= Time.deltaTime;
        }
        else if (jumping)
        {
            curJumpTime -= Time.deltaTime;
            if (curJumpTime < 0 || !target || Vector3.Distance(target.position, transform.position) < 1.75f)
                animator.CrossFade("SlimeLanding", 0, 0);
        }
        else if (target)
        {
            animator.CrossFade("SlimeBounce", 0, 0);
            targetPos = target.position;
            Movement(targetPos);
            bc.excludeLayers = wallLayer;
            sr.enabled = true;
            curJumpTime = jumpTime;
            jumping = true;
        }
    }

    private void FixedUpdate()
    {
        if (jumping) return;
        rb.linearVelocity = Vector3.zero;
    }

    public void OnLanding()
    {
        sr.enabled = false;
        bc.excludeLayers = new ();
        jumping = false;
        curJumpCD = jumpCD;

        if (!target) return;
        if (Vector2.Distance(target.position, transform.position) > 1) return;
        target.GetComponent<Health>()?.TakeDamage(atk, Element.Water);
    }
}
