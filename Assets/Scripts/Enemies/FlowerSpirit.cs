using System.Collections;
using UnityEngine;

public class FlowerSpirit : Enemy
{
    [SerializeField] float rootDistance = 8f;
    [SerializeField] float retreatDistance = 5f;
    [SerializeField] float retreatDistanceFloor = 2f;
    [SerializeField] float rootingSpeed = 3f;
    [SerializeField] float repositionThreshold = 0.5f;
    [SerializeField] float sinkDuration = 0.5f;
    [SerializeField] float emergeDuration = 0.5f;
    [SerializeField] float attackCooldown = 4f;
    [SerializeField] float chargeDuration = 2f;
    [SerializeField] float beamDuration = 1.5f;
    [SerializeField] float beamDamageTickRate = 0.3f;
    [SerializeField] ParticleSystem undergroundTrail;
    [SerializeField] LineRenderer beamRenderer;

    readonly int sinkAnim = Animator.StringToHash("FlowerSinking");
    readonly int emergeAnim = Animator.StringToHash("FlowerEmerging");
    readonly int chargeAnim = Animator.StringToHash("FlowerCharging");
    readonly int firingAnim = Animator.StringToHash("FlowerFiring");

    protected override int IdleAnim => Animator.StringToHash("FlowerIdle");

    ForestSpiritState state;
    Vector2 rootPosition, beamDirection;
    Health health;
    float currentCooldown;

    new void Start()
    {
        base.Start();
        state = ForestSpiritState.Idle;
        currentCooldown = attackCooldown;
        health = GetComponent<Health>();
    }

    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;

        switch (state)
        {
            case ForestSpiritState.Idle:
                if (Vector2.Distance(transform.position, target.position) < retreatDistance)
                {
                    CalculateRootPosition();
                    state = ForestSpiritState.Sinking;
                    animator.CrossFade(sinkAnim, 0, 0);
                    break;
                }
                currentCooldown -= Time.deltaTime;

                if (currentCooldown > 0) break;
                StartBeamAttack();
                break;
        }
    }

    void FixedUpdate()
    {
        if (state == ForestSpiritState.Rooting)
        {
            Vector2 dir = (rootPosition - (Vector2)transform.position).normalized;
            rigidbody.linearVelocity = dir * rootingSpeed;
            return;
        }
        rigidbody.linearVelocity = Vector2.zero;
    }

    void CalculateRootPosition()
    {
        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)target.position).normalized;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, awayFromPlayer, rootDistance, LayerMask.GetMask("Wall"));
        float magnitude = raycastHit2D ? (raycastHit2D.point - (Vector2)transform.position).magnitude : rootDistance;

        if (magnitude < retreatDistanceFloor) return;
        rootPosition = (Vector2)target.position + awayFromPlayer * magnitude;
    }

    void FinishSinking()
    {
        if (IsStaggered) return;
        StartCoroutine(RootSequence());
    }

    void FinishEmerging()
    {
        if (IsStaggered) return;
        state = ForestSpiritState.Idle;
        animator.CrossFade(IdleAnim, 0, 0);
    }

    IEnumerator RootSequence()
    {
        state = ForestSpiritState.Rooting;
        health.SetInvincibility(true);
        undergroundTrail.Play();

        while (Vector2.Distance(transform.position, rootPosition) > repositionThreshold)
        {
            if (IsStaggered) break;
            yield return null;
        }

        state = ForestSpiritState.Emerging;
        health.SetInvincibility(false);
        undergroundTrail.Stop();
        rigidbody.linearVelocity = Vector2.zero;
        animator.CrossFade(emergeAnim, 0, 0);
    }

    void StartBeamAttack()
    {
        state = ForestSpiritState.Charging;
        beamDirection = target ? ((Vector2)target.position - (Vector2)transform.position).normalized : Vector2.right;
        animator.CrossFade(chargeAnim, 0, 0);
    }

    void FinishCharging()
    {
        if (IsStaggered) return;
        StartCoroutine(BeamAttack());
    }

    IEnumerator BeamAttack()
    {
        state = ForestSpiritState.Firing;
        animator.CrossFade(firingAnim, 0, 0);
        yield return StartCoroutine(FireBeam());

        if (IsStaggered) yield break;
        state = ForestSpiritState.Idle;
        animator.CrossFade(IdleAnim, 0, 0);
        currentCooldown = attackCooldown;
    }

    IEnumerator FireBeam()
    {
        beamRenderer.enabled = true;
        float elapsed = 0f;
        float tickTimer = 0f;

        while (elapsed < beamDuration && !IsStaggered)
        {
            UpdateBeamVisual();
            tickTimer += Time.deltaTime;
            if (tickTimer >= beamDamageTickRate)
            {
                DealBeamDamage();
                tickTimer = 0f;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        beamRenderer.enabled = false;
    }

    void UpdateBeamVisual()
    {
        beamRenderer.SetPosition(0, Vector3.back);
        beamRenderer.SetPosition(1, (Vector3)beamDirection * 20f + Vector3.back);
    }

    void DealBeamDamage()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, beamDirection);
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider.GetComponent<Player>()) continue;
            hit.collider.GetComponent<Health>().TakeDamage(attack, Element.Nature, transform.position);
            beamRenderer.SetPosition(1, hit.transform.position - transform.position + Vector3.back);
            break;
        }
    }

    public override void OnStagger()
    {
        base.OnStagger();
        if (beamRenderer) beamRenderer.enabled = false;
        if (undergroundTrail) undergroundTrail.Stop();
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = ForestSpiritState.Idle;
        currentCooldown = attackCooldown;
    }
}

enum ForestSpiritState { Idle, Sinking, Rooting, Emerging, Charging, Firing }
