using System.Linq;
using UnityEngine;

public class RockySlime : Enemy
{
    [SerializeField] int projectileCount;
    [SerializeField] float jumpTime, attackCooldown, jumpThreshold, minProjectileDistance, maxProjectileDistance, projectileAngleRange;
    [SerializeField] GameObject prefabNatureProjectile, prefabEarthProjectile;

    float currentJumpTime, currentAttackCooldown;
    int landingAnim = Animator.StringToHash("SlimeLanding"),
        launchingAnim = Animator.StringToHash("RockyBurst"),
        bounceAnim = Animator.StringToHash("SlimeBounce");

    SpriteRenderer spriteRenderer;
    Shooter shooter;
    RockySlimeState rockyState;
    Vector2 targetPos;

    protected override int IdleAnim => Animator.StringToHash("SlimeIdle");

    new void Start()
    {
        base.Start();

        currentAttackCooldown = 0;
        currentJumpTime = 0;
        rockyState = RockySlimeState.Idle;
        targetPos = Vector2.zero;

        spriteRenderer = GetComponent<SpriteRenderer>();
        shooter = GetComponent<Shooter>();

        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (IsStaggered) return;

        if (!target) SearchTarget(transform.position, detectDistance);

        switch (rockyState)
        {
            case RockySlimeState.Jumping:
                currentJumpTime -= Time.deltaTime;
                Movement(targetPos);

                if (currentJumpTime < 0 || !target || Vector3.Distance(targetPos, transform.position) < 1.75f || IsStaggered)
                    animator.CrossFade(landingAnim, 0, 0);
                break;
            case RockySlimeState.Idle:
                rigidbody.linearVelocity = Vector2.zero;
                currentAttackCooldown -= Time.deltaTime;

                if (currentAttackCooldown > 0) break;
                if (!target) break;
                if (Vector2.Distance(transform.position, target.position) >= jumpThreshold) Jump();
                else Attack();
                break;
        }
    }

    void Jump()
    {
        animator.CrossFade(bounceAnim, 0, 0);
        targetPos = target.position;
        spriteRenderer.enabled = true;
        currentJumpTime = jumpTime;
        rockyState = RockySlimeState.Jumping;
        SetInvincibility(true);
    }

    void Attack()
    {
        animator.CrossFade(launchingAnim, 0, 0);
        rockyState = RockySlimeState.Attacking;
    }

    void ResetToIdle()
    {
        SetInvincibility(false);
        spriteRenderer.enabled = false;
        rigidbody.linearVelocity = Vector2.zero;
        rockyState = RockySlimeState.Idle;
        currentAttackCooldown = attackCooldown;
        animator.CrossFade(IdleAnim, 0, 0);
    }

    void DealProjectileDamage(Collider2D[] colliders, Projectile projectile, Element element)
    {
        if (colliders.Count(collider => collider.transform == target) == 0) return;
        int damage = Mathf.RoundToInt(attack * 0.8f);
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth) targetHealth.TakeDamage(damage, element, projectile.transform.position);

        if (!projectile.gameObject) return;
        Destroy(projectile.gameObject);
    }

    public void OnLanding()
    {
        if (IsStaggered) return;
        spriteRenderer.enabled = false;
        ResetToIdle();

        if (!target) return;
        if (Vector2.Distance(target.position, transform.position) > 1.5f) return;
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth) targetHealth.TakeDamage(attack, Element.Nature, transform.position);
    }

    public void FireProjectiles()
    {
        if (IsStaggered) return;
        bool projectileRandom = Random.value > 0.5f;
        float targetAngle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x);
        GameObject prefabProjectile = projectileRandom ? prefabEarthProjectile : prefabNatureProjectile;
        Element element = projectileRandom ? Element.Earth : Element.Nature;

        for (float i = 0; i < projectileCount; i++)
        {
            float magnitude = Random.Range(minProjectileDistance, maxProjectileDistance);
            float baseAngle = i / projectileCount * 2 * Mathf.PI;
            float range = projectileAngleRange * Mathf.PI / 180;
            float angleOffset = Random.Range(-range, range);
            float angle = targetAngle + angleOffset + baseAngle;
            Vector3 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 targetPosition = direction * magnitude + transform.position;

            shooter.FireProjectile(prefabProjectile, targetPosition, out Projectile projectile, FiringMode.FirePoint);
            projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) => DealProjectileDamage(colliders, projectile, element));
        }
    }

    public void FinishAttack()
    {
        if (IsStaggered) return;
        ResetToIdle();
    }

    public override void OnStagger()
    {
        base.OnStagger();
        spriteRenderer.enabled = false;
        SetInvincibility(false);
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        rockyState = RockySlimeState.Idle;
        currentAttackCooldown = 0;
    }


    public enum RockySlimeState { None, Idle, Jumping, Attacking }
}
