using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] float jumpTime, jumpCD;

    readonly int landingAnim = Animator.StringToHash("SlimeLanding"),
                 bounceAnim = Animator.StringToHash("SlimeBounce");

    float curJumpTime, curJumpCD;
    SlimeState slimeState;

    SpriteRenderer spriteRenderer;
    Vector2 targetPos;

    protected override int IdleAnim => Animator.StringToHash("SlimeIdle");


    new void Start()
    {
        base.Start();

        curJumpCD = 0;
        curJumpTime = 0;
        slimeState = SlimeState.Idle;

        targetPos = Vector2.zero;

        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (IsStaggered) return;
        SearchTarget(transform.position, detectDistance);

        switch (slimeState)
        {
            case SlimeState.Idle:
                if (curJumpCD > 0) { curJumpCD -= Time.deltaTime; rigidbody.linearVelocity = Vector2.zero; break; }
                if (!target) break;
                Jump();
                break;
            case SlimeState.Jumping:
                curJumpTime -= Time.deltaTime;
                Movement(targetPos);
                if (curJumpTime < 0 || !target || Vector3.Distance(targetPos, transform.position) < 1.75f || IsStaggered)
                    animator.CrossFade(landingAnim, 0, 0);
                break;
        }
    }

    void Jump()
    {
        animator.CrossFade(bounceAnim, 0, 0);
        targetPos = target.position;
        Movement(targetPos);
        spriteRenderer.enabled = true;
        curJumpTime = jumpTime;
        slimeState = SlimeState.Jumping;
    }

    public void OnLanding()
    {
        if (IsStaggered) return;
        spriteRenderer.enabled = false;
        rigidbody.linearVelocity = Vector2.zero;
        slimeState = SlimeState.Idle;
        animator.CrossFade(IdleAnim, 0, 0);
        curJumpCD = jumpCD;

        if (!target) return;
        if (Vector2.Distance(target.position, transform.position) > 1) return;
        Health health = target.GetComponent<Health>();
        if (health) health.TakeDamage(attack, Element.Water, transform.position);
    }

    public override void OnStagger()
    {
        base.OnStagger();
        spriteRenderer.enabled = false;
    }

    public override void OnStaggerEnd()
    {
        base.OnStaggerEnd();
        slimeState = SlimeState.Idle;
        curJumpCD = 0;
    }
}

enum SlimeState { Idle, Jumping }
