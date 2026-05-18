using System.Collections;
using UnityEngine;

public class FlowerSpirit : Enemy
{
    [SerializeField] float rootDistance = 8f;
    [SerializeField] float rootingSpeed = 3f;
    [SerializeField] float repositionThreshold = 0.5f;
    [SerializeField] float attackCooldown = 4f;
    [SerializeField] float chargeDuration = 2f;
    [SerializeField] float beamDuration = 1.5f;
    [SerializeField] float beamDamageTickRate = 0.3f;
    [SerializeField] LineRenderer beamRenderer;

    readonly int chargeAnim = Animator.StringToHash("ForestSpiritCharge");
    readonly int firingAnim = Animator.StringToHash("ForestSpiritFiring");

    protected override int IdleAnim => Animator.StringToHash("ForestSpiritIdle");

    ForestSpiritState state;
    Vector2 rootPosition;
    Vector2 beamDirection;
    float currentCooldown;

    new void Start()
    {
        base.Start();
        state = ForestSpiritState.Idle;
        currentCooldown = attackCooldown;
    }

    void Update()
    {
        if (state == ForestSpiritState.Staggered) return;

        SearchTarget(transform.position, detectDistance);

        if (!target) return;

        switch (state)
        {
            case ForestSpiritState.Idle:
                CalculateRootPosition();
                state = ForestSpiritState.Rooting;
                break;

            case ForestSpiritState.Rooting:
                if (Vector2.Distance(transform.position, rootPosition) <= repositionThreshold)
                    EnterRooted();
                break;

            case ForestSpiritState.Rooted:
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0)
                    StartBeamAttack();
                break;
        }
    }

    void FixedUpdate()
    {
        if (state == ForestSpiritState.Rooting)
        {
            Vector2 dir = (rootPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * rootingSpeed;
            return;
        }
        rb.linearVelocity = Vector2.zero;
    }

    void CalculateRootPosition()
    {
        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)target.position).normalized;
        rootPosition = (Vector2)target.position + awayFromPlayer * rootDistance;
    }

    void EnterRooted()
    {
        state = ForestSpiritState.Rooted;
        animator.CrossFade(IdleAnim, 0, 0);
    }

    void StartBeamAttack()
    {
        state = ForestSpiritState.Charging;
        beamDirection = target ? ((Vector2)target.position - (Vector2)transform.position).normalized : Vector2.right;
        animator.CrossFade(chargeAnim, 0, 0);
        StartCoroutine(BeamAttack());
    }

    IEnumerator BeamAttack()
    {
        yield return new WaitForSeconds(chargeDuration);
        if (IsStaggered) yield break;

        state = ForestSpiritState.Firing;
        animator.CrossFade(firingAnim, 0, 0);
        yield return StartCoroutine(FireBeam());

        if (IsStaggered) yield break;
        state = ForestSpiritState.Rooted;
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
            hit.collider.GetComponent<Health>().TakeDamage(atk, Element.Nature, transform.position);
            beamRenderer.SetPosition(1, hit.transform.position - transform.position + Vector3.back);
            break;
        }
    }

    public override void OnStagger()
    {
        base.OnStagger();
        state = ForestSpiritState.Staggered;
        if (beamRenderer) beamRenderer.enabled = false;
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = ForestSpiritState.Rooted;
        currentCooldown = attackCooldown;
    }
}

enum ForestSpiritState { Idle, Rooting, Rooted, Charging, Firing, Staggered }
