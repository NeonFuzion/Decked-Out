using System.Collections.Generic;
using UnityEngine;

public class TreeSpirit : Enemy
{
    [Header("Combat")]
    [SerializeField] float attackCooldown = 5f;

    [Header("Physical Slam")]
    [SerializeField] float meleeRange = 2.5f;
    [SerializeField] float slamRadius = 2.5f;
    [SerializeField] ParticleSystem slamEffect;

    [Header("Nature Attack")]
    [SerializeField] float natureAttackRange = 8f;
    [SerializeField] GameObject prefabNatureProjectile;
    [SerializeField] int projectileCount = 3;
    [SerializeField] float projectileSpreadAngle = 25f;

    [Header("Nature Heal")]
    [SerializeField] float healRange = 6f;
    [SerializeField] int healAmount = 30;
    [SerializeField] float groupingOffset = 2f;
    [SerializeField] ParticleSystem healAuraEffect;

    readonly int walkAnim = Animator.StringToHash("TreeSpiritWalk");
    readonly int windUpAnim = Animator.StringToHash("TreeSpiritWindUp");
    readonly int slamAnim = Animator.StringToHash("TreeSpiritSlam");
    readonly int natureAttackAnim = Animator.StringToHash("TreeSpiritNatureAttack");
    readonly int healAnim = Animator.StringToHash("TreeSpiritHeal");

    protected override int IdleAnim => Animator.StringToHash("TreeSpiritIdle");

    TreeSpiritState state;
    TreeSpiritAbility pendingAbility;
    float currentCooldown;
    Shooter shooter;
    bool wasMoving;

    new void Start()
    {
        base.Start();
        state = TreeSpiritState.Idle;
        currentCooldown = attackCooldown;
        shooter = GetComponent<Shooter>();
    }

    void Update()
    {
        SearchTarget(transform.position, detectDistance);

        if (state != TreeSpiritState.Idle) return;

        bool isMoving = target != null && !IsStaggered;
        if (isMoving != wasMoving)
        {
            animator.CrossFade(isMoving ? walkAnim : IdleAnim, 0, 0);
            wasMoving = isMoving;
        }

        if (!target)
        {
            rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Movement();
        currentCooldown -= Time.deltaTime;

        if (currentCooldown <= 0)
            SelectAbility();
    }

    void SelectAbility()
    {
        if (!target) return;

        float distToPlayer = Vector2.Distance(transform.position, target.position);
        List<TreeSpiritAbility> pool = new();

        if (distToPlayer <= meleeRange)
            pool.Add(TreeSpiritAbility.Physical);

        pool.Add(TreeSpiritAbility.NatureAttack);

        if (HasNearbyAllies())
            pool.Add(TreeSpiritAbility.NatureHeal);

        pendingAbility = pool[Random.Range(0, pool.Count)];
        BeginWindUp();
    }

    void BeginWindUp()
    {
        state = TreeSpiritState.WindingUp;
        rigidbody.linearVelocity = Vector2.zero;
        animator.CrossFade(windUpAnim, 0, 0);
    }

    // Animation event: end of wind-up animation
    public void ExecuteAbility()
    {
        if (IsStaggered) return;

        state = TreeSpiritState.Executing;

        switch (pendingAbility)
        {
            case TreeSpiritAbility.Physical:     animator.CrossFade(slamAnim, 0, 0);        break;
            case TreeSpiritAbility.NatureAttack: animator.CrossFade(natureAttackAnim, 0, 0); break;
            case TreeSpiritAbility.NatureHeal:   animator.CrossFade(healAnim, 0, 0);         break;
        }
    }

    // Animation event: slam impact frame
    public void SlamHit()
    {
        if (IsStaggered) return;

        if (slamEffect) slamEffect.Play();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, slamRadius))
        {
            if (!col.GetComponent<Player>()) continue;
            col.GetComponent<Health>()?.TakeDamage(attack, Element.Physical, transform.position);
        }
    }

    // Animation event: nature projectile fire frame
    public void FireNatureProjectiles()
    {
        if (IsStaggered || !target || !prefabNatureProjectile) return;

        float baseAngle = Mathf.Atan2(
            target.position.y - transform.position.y,
            target.position.x - transform.position.x
        );
        float halfSpread = projectileSpreadAngle * 0.5f * Mathf.Deg2Rad;

        for (int i = 0; i < projectileCount; i++)
        {
            float t = projectileCount == 1 ? 0f : (float)i / (projectileCount - 1) - 0.5f;
            float angle = baseAngle + t * halfSpread * 2f;
            Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 targetPos = (Vector2)transform.position + dir * natureAttackRange;

            shooter.FireProjectile(prefabNatureProjectile, targetPos, out Projectile projectile, FiringMode.FirePoint);
            projectile.OnHit.AddListener((cols, proj) => DealNatureProjectileDamage(cols, proj));
        }
    }

    void DealNatureProjectileDamage(Collider2D[] colliders, Projectile projectile)
    {
        foreach (Collider2D col in colliders)
        {
            if (!col.GetComponent<Player>()) continue;
            col.GetComponent<Health>()?.TakeDamage(attack, Element.Nature, projectile.transform.position);
            if (projectile) Destroy(projectile.gameObject);
            return;
        }
    }

    // Animation event: heal pulse frame
    public void HealAllies()
    {
        if (IsStaggered) return;

        if (healAuraEffect) healAuraEffect.Play();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, healRange))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (!enemy || enemy == this) continue;
            col.GetComponent<Health>()?.Heal(healAmount);
        }

        health.Heal(healAmount / 2);
    }

    // Animation event: end of any attack animation
    public void FinishAbility()
    {
        animator.CrossFade(IdleAnim, 0, 0);
        
        if (IsStaggered) return;
        state = TreeSpiritState.Idle;
        currentCooldown = attackCooldown;
        wasMoving = false;
    }

    // Other enemies can call this to find the rally position behind the Tree Spirit
    public Vector2 GetGroupingPosition()
    {
        if (!target) return transform.position;
        Vector2 away = ((Vector2)transform.position - (Vector2)target.position).normalized;
        return (Vector2)transform.position + away * groupingOffset;
    }

    bool HasNearbyAllies()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, healRange))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy && enemy != this) return true;
        }
        return false;
    }

    public override void OnStagger()
    {
        base.OnStagger();
        if (slamEffect) slamEffect.Stop();
        if (healAuraEffect) healAuraEffect.Stop();
        wasMoving = false;
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = TreeSpiritState.Idle;
        currentCooldown = attackCooldown * 0.5f;
    }

    enum TreeSpiritState { Idle, WindingUp, Executing }
    enum TreeSpiritAbility { Physical, NatureAttack, NatureHeal }
}
