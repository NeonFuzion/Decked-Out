using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMachine : Enemy
{
    [SerializeField] LineRenderer laser;

    int idleAnim = Animator.StringToHash("CubeMachineIdle"),
        activeAnim = Animator.StringToHash("CubeMachineActive"),
        firingAnim = Animator.StringToHash("CubeMachineFiring");
    bool inAtkingRange;

    Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Health>().Initialize(hp, def);
    }

    // Update is called once per frame
    void Update()
    {
        SearchTarget(transform.position, detectDistance);

        if (!target) return;
        inAtkingRange = Vector2.Distance(target.position, transform.position) < 6;

        animator.CrossFade(AnimationHelper(), 0, 0);
    }

    private void FixedUpdate()
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
        if (!target) return;
        direction = (target.position - transform.position).normalized;
    }

    void FireLaser()
    {
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
            hit.collider.GetComponent<Health>().TakeDamage(atk, Element.Fire);
            break;
        }

        laser.enabled = true;
        yield return new WaitForSeconds(0.02f);
        laser.enabled = false;
    }

    int AnimationHelper()
    {
        if (inAtkingRange) return firingAnim;
        if (target) return activeAnim;
        return idleAnim;
    }
}
