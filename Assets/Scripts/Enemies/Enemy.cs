using UnityEngine;

public abstract class Enemy : Being
{
    [SerializeField] protected int attack, movementSpeed, detectDistance;
    [SerializeField] protected Animator animator;

    protected new Rigidbody2D rigidbody;
    protected Transform target;
    protected Health health;
    protected Stagger stagger;

    protected abstract int IdleAnim { get; }

    public bool IsStaggered { get; private set; }
    public bool IsImmobile { get; private set; }

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;

        rigidbody = GetComponent<Rigidbody2D>();
        stagger = GetComponent<Stagger>();
        health = GetComponent<Health>();
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
        if (target && Vector2.Distance(target.position, transform.position) <= detectDistance) return;
        target = FindPlayer(detectPoint, radius);
    }

    protected void Movement()
    {
        if (!target) return;
        Movement(target.position);
    }

    protected void Movement(Vector3 targetPosition)
    {
        if (IsStaggered) return;
        if (IsImmobile) return;
        Vector2 direction = (targetPosition - transform.position).normalized;
        rigidbody.linearVelocity = direction * movementSpeed;
    }

    protected void SetInvincibility(bool isInvincible)
    {
        health.SetInvincibility(isInvincible);
        stagger.SetInvincibility(isInvincible);
    }

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

    public void OnKnockbackStart()
    {
        IsImmobile = true;
        rigidbody.linearVelocity = Vector2.zero;
    }

    public void OnKnockbackEnd()
    {
        IsImmobile = false;
        rigidbody.linearVelocity = Vector2.zero;
    }
}
