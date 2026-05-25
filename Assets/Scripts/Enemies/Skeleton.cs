using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    int activeAnim = Animator.StringToHash("SkeletonActive"),
        throwingAnim = Animator.StringToHash("SkeletonBoneToss");

    SkeletonState state;
    Animator animr;
    Vector2 direction;

    protected override int IdleAnim => Animator.StringToHash("SkeletonIdle");


    new void Start()
    {
        base.Start();
        state = SkeletonState.Idle;
        animr = GetComponent<Animator>();
    }

    void Update()
    {
        if (state == SkeletonState.Staggered) return;

        SearchTarget(transform.position, detectDistance);

        if (!target) state = SkeletonState.Idle;
        else if (Vector2.Distance(target.position, transform.position) < 6) state = SkeletonState.Throwing;
        else state = SkeletonState.Active;

        animr.CrossFade(AnimationHelper(), 0, 0);
    }

    void FixedUpdate()
    {
        if (state != SkeletonState.Active)
        {
            rigidbody.linearVelocity = Vector2.zero;
            return;
        }
        Movement();
    }

    void LockTargetPos()
    {
        if (IsStaggered) return;
        direction = (target.position - transform.position).normalized;
    }

    void ThrowBone()
    {
        if (IsStaggered) return;
    }

    int AnimationHelper()
    {
        return state switch
        {
            SkeletonState.Throwing => throwingAnim,
            SkeletonState.Active => activeAnim,
            _ => IdleAnim,
        };
    }

    public override void OnStagger()
    {
        base.OnStagger();
        state = SkeletonState.Staggered;
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = SkeletonState.Idle;
    }

}

enum SkeletonState { Idle, Active, Throwing, Staggered }
