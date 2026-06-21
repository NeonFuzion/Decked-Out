using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMachine : Enemy
{
    [SerializeField] float firingDistance = 6;
    [SerializeField] LineRenderer laser;

    int activeAnim = Animator.StringToHash("CubeMachineActive"),
        firingAnim = Animator.StringToHash("CubeMachineFiring");

    CubeMachineState state;
    Vector2 direction;

    protected override int IdleAnim  => Animator.StringToHash("CubeMachineIdle");

    new void Start()
    {
        base.Start();
        state = CubeMachineState.Idle;
    }

    void Update()
    {
        if (IsStaggered) return;
        SearchTarget(transform.position, detectDistance);

        if (!target) state = CubeMachineState.Idle;
        else if (Vector2.Distance(target.position, transform.position) < firingDistance) state = CubeMachineState.Firing;
        else state = CubeMachineState.Active;

        animator.CrossFade(AnimationHelper(), 0, 0);
    }

    private void FixedUpdate()
    {
        if (state != CubeMachineState.Active)
        {
            rigidbody.linearVelocity = Vector2.zero;
            return;
        }
        Movement();
    }

    void LockTargetPos()
    {
        if (IsStaggered || !target) return;
        direction = (target.position - transform.position).normalized;
    }

    void FireLaser()
    {
        if (IsStaggered) return;
        StartCoroutine(RenderLaser());
    }

    IEnumerator RenderLaser()
    {
        RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, direction);

        laser.SetPosition(0, Vector3.back);
        laser.SetPosition(1, (Vector3)direction * 100 + Vector3.back);
        foreach (RaycastHit2D hit in hitList)
        {
            if (!hit.collider.GetComponent<Player>()) continue;
            Vector3 laserEndPoint = hit.transform.position - transform.position;
            laser.SetPosition(1, laserEndPoint + Vector3.back);
            hit.collider.GetComponent<Health>().TakeDamage(attack, Element.Fire, transform.position);
            break;
        }

        laser.enabled = true;
        yield return new WaitForSeconds(0.02f);
        laser.enabled = false;
    }

    int AnimationHelper()
    {
        return state switch
        {
            CubeMachineState.Firing => firingAnim,
            CubeMachineState.Active => activeAnim,
            _ => IdleAnim,
        };
    }

    public override void OnStagger()
    {
        base.OnStagger();
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = CubeMachineState.Idle;
    }
}

enum CubeMachineState { Idle, Active, Firing }
