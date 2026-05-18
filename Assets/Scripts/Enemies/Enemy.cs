using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Being
{
    [SerializeField] protected int atk, spd, detectDistance;
    [SerializeField] protected Animator animator;

    protected Rigidbody2D rb;
    protected Transform target;

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected abstract int IdleAnim { get; }

    protected Transform FindPlayer(Vector2 detectPoint, int radius)
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(detectPoint, radius))
        {
            Player script = col.GetComponent<Player>();

            if (!script) continue;
            return col.transform;
        }
        return null;
    }

    protected void SearchTarget(Vector2 detectPoint, int radius)
    {
        target = FindPlayer(detectPoint, radius);
    }

    protected void Movement()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * spd;
    }

    protected void Movement(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * spd;
    }

    public bool IsStaggered { get; private set; }

    public virtual void OnStagger()
    {
        IsStaggered = true;
        StopAllCoroutines();
        animator.CrossFade(IdleAnim, 0, 0);
        rb.linearVelocity = Vector2.zero;
    }

    public virtual void OnStaggerEnd()
    {
        IsStaggered = false;
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
