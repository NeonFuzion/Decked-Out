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

    FlowerSpiritState state;
    Vector2 rootPosition, beamDirection;
    float currentCooldown;

    new void Start()
    {
        base.Start();
        state = FlowerSpiritState.Idle;
        currentCooldown = attackCooldown;
        health = GetComponent<Health>();
    }

    void Update()
    {
        SearchTarget(transform.position, detectDistance);
        if (!target) return;

        switch (state)
        {
            case FlowerSpiritState.Idle:
                currentCooldown -= Time.deltaTime;

                if (Vector2.Distance(transform.position, target.position) < retreatDistance)
                {
                    bool hasSpace = CalculateRootPosition();

                    if (!hasSpace) break;
                    state = FlowerSpiritState.Sinking;
                    animator.CrossFade(sinkAnim, 0, 0);
                    break;
                }

                if (currentCooldown > 0) break;
                StartBeamAttack();
                break;
        }
    }

    void FixedUpdate()
    {
        if (state == FlowerSpiritState.Rooting)
        {
            Vector2 dir = (rootPosition - (Vector2)transform.position).normalized;
            rigidbody.linearVelocity = dir * rootingSpeed;
            return;
        }
        rigidbody.linearVelocity = Vector2.zero;
    }

    bool CalculateRootPosition()
    {
        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)target.position).normalized;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, awayFromPlayer, rootDistance, LayerMask.GetMask("Wall"));
        float magnitude = raycastHit2D ? Vector2.Distance(raycastHit2D.point, transform.position) - 0.4f : rootDistance;

        if (magnitude < retreatDistanceFloor) return false;
        rootPosition = (Vector2)target.position + awayFromPlayer * magnitude;
        return true;
    }

    void FinishSinking()
    {
        if (IsStaggered) return;
        StartCoroutine(RootSequence());
    }

    void FinishEmerging()
    {
        if (IsStaggered) return;
        state = FlowerSpiritState.Idle;
        animator.CrossFade(IdleAnim, 0, 0);
    }

    IEnumerator RootSequence()
    {
        state = FlowerSpiritState.Rooting;
        SetInvincibility(true);
        undergroundTrail.Play();

        while (Vector2.Distance(transform.position, rootPosition) > repositionThreshold)
        {
            if (IsStaggered) break;
            yield return null;
        }

        state = FlowerSpiritState.Emerging;
        SetInvincibility(false);
        undergroundTrail.Stop();
        rigidbody.linearVelocity = Vector2.zero;
        animator.CrossFade(emergeAnim, 0, 0);
    }

    void StartBeamAttack()
    {
        state = FlowerSpiritState.Charging;
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
        state = FlowerSpiritState.Firing;
        animator.CrossFade(firingAnim, 0, 0);
        yield return StartCoroutine(FireBeam());

        if (IsStaggered) yield break;
        state = FlowerSpiritState.Idle;
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
        state = FlowerSpiritState.Idle;
        currentCooldown = attackCooldown;
    }
}

enum FlowerSpiritState { Idle, Sinking, Rooting, Emerging, Charging, Firing }
