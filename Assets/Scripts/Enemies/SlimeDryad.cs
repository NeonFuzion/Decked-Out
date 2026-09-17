using System.Collections;
using UnityEngine;

public class SlimeDryad : Enemy
{
    [Header("Transforms")]
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    [Header("Pound")]
    [SerializeField] float poundRange = 2.5f;

    [Header("Leap")]
    [SerializeField] float leapDuration = 0.6f;
    [SerializeField] float leapLandingDistance = 1.75f;
    [SerializeField] float leapImpactRange = 2.5f;
    [SerializeField] float leapKnockbackMultiplier = 1.75f;

    [Header("Leech")]
    [SerializeField] float leechRange = 4f;
    [SerializeField] float leechCooldown = 6f;
    [SerializeField] float leechDamageMultiplier = 0.5f;
    [SerializeField] float leechHealDuration = 3f;
    [SerializeField] float leechHealTickRate = 0.5f;
    [SerializeField] int leechHealPerTick = 5;

    [Header("Effects")]
    [SerializeField] ParticleSystem slamParticles;
    [SerializeField] ParticleSystem leechingParticles;

    readonly int leapAnim = Animator.StringToHash("SlimeDryadLeap");
    readonly int leapLandAnim = Animator.StringToHash("SlimeDryadLand");
    readonly int poundWindUpAnim = Animator.StringToHash("SlimeDryadPoundWindUp");
    readonly int poundAnim = Animator.StringToHash("SlimeDryadPound");
    readonly int leechWindUpAnim = Animator.StringToHash("SlimeDryadLeechWindUp");
    readonly int leechEndAnim = Animator.StringToHash("SlimeDryadLeechEnd");

    protected override int IdleAnim => Animator.StringToHash("SlimeDryadIdle");

    SlimeDryadState state;
    SlimeDryadAbility pendingAbility;
    Vector2 leapTargetPos;
    float leapTimer, leechCooldownTimer;

    new void Start()
    {
        base.Start();
        state = SlimeDryadState.Idle;
        leechCooldownTimer = 0;
    }

    void Update()
    {
        SearchTarget(transform.position, detectDistance);

        switch (state)
        {
            case SlimeDryadState.Idle:
                UpdateIdle();
                break;
            case SlimeDryadState.Leaping:
                UpdateLeap();
                break;
            case SlimeDryadState.WindingUp:
                UpdateWindup();
                break;
        }
    }

    void UpdateIdle()
    {
        if (IsStaggered) return;
        if (!target)
        {
            movementScript.SetMovement(Vector2.zero);
            return;
        }

        transform.localScale = new(Mathf.Sign(transform.position.x - target.position.x), 1);
        leechCooldownTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, target.position);

        if (leechCooldownTimer <= 0 && distToPlayer <= leechRange)
        {
            BeginWindUp(SlimeDryadAbility.Leech);
        }
        else if (distToPlayer > poundRange)
        {
            BeginLeap();
        }
        else
        {
            BeginWindUp(SlimeDryadAbility.Pound);
        }
    }

    void UpdateLeap()
    {
        leapTimer -= Time.deltaTime;
        MovementToPosition(leapTargetPos);

        if (leapTimer < 0 || !target || Vector2.Distance(leapTargetPos, transform.position) < leapLandingDistance || IsStaggered)
            animator.CrossFade(leapLandAnim, 0, 0);
    }

    void UpdateWindup()
    {
        if (pendingAbility != SlimeDryadAbility.Leech) return;
        if (!target) return;
        Vector2 direction = target.position - transform.position;
        rightHand.up = direction;
    }

    void BeginLeap()
    {
        animator.CrossFade(leapAnim, 0, 0);
        leapTargetPos = target.position;
        leapTimer = leapDuration;
        state = SlimeDryadState.Leaping;
        SetInvincibility(true);
        movementScript.SetMobile();
    }

    // Animation event: end of leap landing animation
    public void OnLeapLanding()
    {
        if (IsStaggered) return;
        SetInvincibility(false);
        movementScript.SetImmobile();
        state = SlimeDryadState.Idle;
        animator.CrossFade(IdleAnim, 0, 0);

        if (!target) return;
        if (Vector2.Distance(target.position, transform.position) > leapImpactRange) return;
        slamParticles.Play();
        DealDamage(target.gameObject, attack, Element.Physical, transform.position, Mathf.RoundToInt(knockback * leapKnockbackMultiplier));
    }

    void BeginWindUp(SlimeDryadAbility ability)
    {
        pendingAbility = ability;
        state = SlimeDryadState.WindingUp;
        movementScript.SetMovement(Vector2.zero);
        animator.CrossFade(ability == SlimeDryadAbility.Pound ? poundWindUpAnim : leechWindUpAnim, 0, 0);
    }

    // Animation event: end of wind-up animation
    public void ExecuteAbility()
    {
        if (IsStaggered) return;
        state = SlimeDryadState.Executing;
        animator.CrossFade(pendingAbility == SlimeDryadAbility.Pound ? poundAnim : leechEndAnim, 0, 0);
    }

    // Animation event: pound impact frame
    public void PoundHit()
    {
        if (IsStaggered || !target) return;
        slamParticles.Play();
        if (Vector2.Distance(transform.position, target.position) > poundRange) return;
        DealDamage(target.gameObject, attack, Element.Physical, transform.position, knockback);
    }

    // Animation event: leech stab frame
    public void LeechStab()
    {
        if (IsStaggered || !target) return;
        if (Vector2.Distance(transform.position, target.position) > leechRange) return;
        int damage = Mathf.RoundToInt(attack * leechDamageMultiplier);
        DealDamage(target.gameObject, damage, Element.Nature, transform.position, knockback);
        StartCoroutine(LeechHealChannel());
    }

    void StartLeechWindup()
    {
        
    }

    void EndLeechWindup()
    {
        
    }

    IEnumerator LeechHealChannel()
    {
        leechingParticles.Play();
        float elapsed = 0f;
        float tickTimer = 0f;

        while (elapsed < leechHealDuration)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= leechHealTickRate)
            {
                health.Heal(leechHealPerTick);
                tickTimer = 0f;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        leechingParticles.Stop();
        animator.CrossFade(leechEndAnim, 0, 0);
    }

    // Animation event: end of pound/leech animation
    public void FinishAbility()
    {
        animator.CrossFade(IdleAnim, 0, 0);

        if (IsStaggered) return;
        if (pendingAbility == SlimeDryadAbility.Leech) leechCooldownTimer = leechCooldown;
        state = SlimeDryadState.Idle;
    }

    public override void OnStagger()
    {
        base.OnStagger();
        SetInvincibility(false);
        leechingParticles.Stop();
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        state = SlimeDryadState.Idle;
    }

    enum SlimeDryadState { Idle, Leaping, WindingUp, Executing }
    enum SlimeDryadAbility { Pound, Leech }
}
