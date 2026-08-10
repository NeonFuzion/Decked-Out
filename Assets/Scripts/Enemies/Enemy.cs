using UnityEngine;

public abstract class Enemy : Being
{
    [SerializeField] protected int attack, detectDistance, knockback;
    [SerializeField] float staggerDefenseMultiplier = 0.5f;
    [SerializeField] protected Transform healthBarTarget;
    [SerializeField] protected Animator animator;

    protected Transform target;
    protected Health health;
    protected Stagger stagger;
    protected Movement movementScript;

    protected abstract int IdleAnim { get; }

    public bool IsStaggered { get; private set; }

    int oldDefense;

    HealthBar healthBar;

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;

        stagger = GetComponent<Stagger>();
        health = GetComponent<Health>();
        movementScript = GetComponent<Movement>();

        healthBar = HealthBarObjectPool.Instance.RetrieveHealthBar(healthBarTarget, health, true, stagger);
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

    protected void DealDamage(GameObject collision, int damage, Element element, Vector2 damageOrigin, int knockback = 1)
    {
        collision.GetComponent<Health>()?.TakeDamage(damage, element, damageOrigin);
        collision.GetComponent<Movement>()?.ApplyKnockback(damageOrigin, knockback);
    }

    public virtual void OnStagger()
    {
        IsStaggered = true;
        StopAllCoroutines();
        animator.CrossFade(IdleAnim, 0, 0);
        movementScript.SetImmobile();
        oldDefense = health.Defense;
        health.SetDefense(Mathf.RoundToInt(oldDefense * staggerDefenseMultiplier));
    }

    public virtual void OnStaggerEnd()
    {
        IsStaggered = false;
        movementScript.SetMobile();
        health.SetDefense(oldDefense);
    }

    public void OnDeath()
    {
        HealthBarObjectPool.Instance.ReturnHealthBar(healthBar, health, stagger);
        Destroy(gameObject);
    }
}
