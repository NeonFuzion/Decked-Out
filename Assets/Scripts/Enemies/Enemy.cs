using UnityEngine;

public abstract class Enemy : Being
{
    [SerializeField] protected int attack, detectDistance;
    [SerializeField] protected Animator animator;

    protected new Rigidbody2D rigidbody;
    protected Transform target;
    protected Health health;
    protected Stagger stagger;
    protected Movement movementScript;

    protected abstract int IdleAnim { get; }

    public bool IsStaggered { get; private set; }

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;

        rigidbody = GetComponent<Rigidbody2D>();
        stagger = GetComponent<Stagger>();
        health = GetComponent<Health>();
        movementScript = GetComponent<Movement>();
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

    protected void MovementToTarget()
    {
        if (!target) return;
        MovementToPosition(target.position);
    }

    protected void MovementToPosition(Vector3 targetPosition)
    {
        movementScript.SetMovementDirection(targetPosition - transform.position);
    }

    protected void Movement(Vector3 movement)
    {
        if (IsStaggered) return;
        movementScript.SetMovement(movement);
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
        movementScript.SetImmobile();
    }

    public virtual void OnStaggerEnd()
    {
        IsStaggered = false;
        movementScript.SetImmobile();
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
