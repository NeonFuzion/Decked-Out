using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Being
{
    [SerializeField] protected int attack, movementSpeed, detectDistance;
    [SerializeField] protected Animator animator;

    protected new Rigidbody2D rigidbody;
    protected Transform target;

    protected abstract int IdleAnim { get; }

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        if (IsStaggered) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rigidbody.linearVelocity = direction * movementSpeed;
    }

    protected void Movement(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rigidbody.linearVelocity = direction * movementSpeed;
    }

    public bool IsStaggered { get; private set; }

    public virtual void OnStagger()
    {
        IsStaggered = true;
        StopAllCoroutines();
        animator.CrossFade(IdleAnim, 0, 0);
        rigidbody.linearVelocity = Vector2.zero;
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
