using System.Linq;
using UnityEngine;

public class RockySlime : Enemy
{
    [SerializeField] int projectileCount;
    [SerializeField] float jumpTime, attackCooldown, jumpThreshold, minProjectileDistance, maxProjectileDistance, projectileAngleRange;
    [SerializeField] ProjectileData natureProjectile, earthProjectile;

    float curJumpTime, currentAttackCooldown;

    BoxCollider2D bc;
    SpriteRenderer sr;
    Shooter shooter;
    RockySlimeState rockyState;
    Vector2 targetPos;
    Health health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();

        currentAttackCooldown = 0;
        curJumpTime = 0;
        rockyState = RockySlimeState.Idle;
        targetPos = Vector2.zero;

        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        shooter = GetComponent<Shooter>();
        health = GetComponent<Health>();

        sr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) SearchTarget(transform.position, detectDistance);
        
        switch (rockyState)
        {
            case RockySlimeState.Jumping:
                curJumpTime -= Time.deltaTime;
                Movement(targetPos);

                if (curJumpTime < 0 || !target || Vector3.Distance(targetPos, transform.position) < 1.75f)
                    animator.CrossFade("SlimeLanding", 0, 0);
                break;
            case RockySlimeState.Idle:
                rb.linearVelocity = Vector2.zero;
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
        animator.CrossFade("SlimeBounce", 0, 0);
        targetPos = target.position;
        bc.excludeLayers = wallLayer;
        sr.enabled = true;
        curJumpTime = jumpTime;
        rockyState = RockySlimeState.Jumping;
        health.SetInvincibility(true);
    }

    void Attack()
    {
        animator.CrossFade("RockyBurst", 0, 0);
        rockyState = RockySlimeState.Attacking;
    }

    void ResetToIdle()
    {
        health.SetInvincibility(false);
        sr.enabled = false;
        rockyState = RockySlimeState.Idle;
        currentAttackCooldown = attackCooldown;
        animator.CrossFade("SlimeIdle", 0, 0);
    }

    void DealProjectileDamage(Collider2D[] colliders, Projectile projectile)
    {
        if (colliders.Count(collider => collider.transform == target) == 0) return;
        int damage = Mathf.RoundToInt(atk * 0.8f);
        Element element = projectile.ProjectileData.ProjectileEffect == natureProjectile ? Element.Nature : Element.Earth;
        target.GetComponent<Health>()?.TakeDamage(damage, element, projectile.transform.position);

        if (!projectile.gameObject) return;
        Destroy(projectile.gameObject);
    }

    public void OnLanding()
    {
        sr.enabled = false;
        bc.excludeLayers = new ();
        ResetToIdle();

        if (!target) return;
        if (Vector2.Distance(target.position, transform.position) > 1.5f) return;
        target.GetComponent<Health>()?.TakeDamage(atk, Element.Nature);
    }

    public void FireProjectiles()
    {
        bool projectileRandom = Random.value > 0.5f;
        float targetAngle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x);
        ProjectileData projectileData = projectileRandom ? earthProjectile : natureProjectile;

        for (float i = 0; i < projectileCount; i++)
        {
            float magnitude = Random.Range(minProjectileDistance, maxProjectileDistance);
            float baseAngle = i / projectileCount * 2 * Mathf.PI;
            float range = projectileAngleRange * Mathf.PI / 180;
            float angleOffset = Random.Range(-range, range);
            float angle = targetAngle + angleOffset + baseAngle;
            Vector3 direction = new (Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 targetPosition = direction * magnitude + transform.position;

            Projectile projectile;
            shooter.FireProjectile(projectileData, targetPosition, out projectile);
            projectile.OnHit.AddListener(DealProjectileDamage);
        }
    }

    public void FinishAttack()
    {
        ResetToIdle();
    }

    public enum RockySlimeState { None, Idle, Jumping, Attacking }
}
