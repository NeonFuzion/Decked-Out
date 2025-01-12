using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    int idleAnim = Animator.StringToHash("SkeletonIdle"),
        activeAnim = Animator.StringToHash("SkeletonActive"),
        throwingAnim = Animator.StringToHash("SkeletonBoneToss");
    bool inAtkingRange;

    Animator animr;
    Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        atk = 20;
        def = 10;
        hp = 70;
        spd = 225;
        detectDistance = 8;

        animr = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GetComponent<Health>().Initialize(hp, def);
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);

        if (!target) return;
        inAtkingRange = Vector2.Distance(target.position, transform.position) < 6;

        animr.CrossFade(AnimationHelper(), 0, 0);
    }

    void FixedUpdate()
    {
        if (inAtkingRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        Movement();
    }

    void LockTargetPos()
    {
        direction = (target.position - transform.position).normalized;
    }

    void ThrowBone()
    {

    }

    int AnimationHelper()
    {
        if (inAtkingRange) return throwingAnim;
        if (target) return activeAnim;
        return idleAnim;
    }
}
