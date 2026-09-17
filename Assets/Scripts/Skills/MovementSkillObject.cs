using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementSkillObject : SkillObject
{
    [SerializeField] UnityEvent onActivate, onDeactivate;

    MovementSkillSO skillSO;
    Movement movementScript;

    bool isActive;
    float nextTickTime, endTick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (Time.time < nextTickTime) return;
        nextTickTime = Time.time + skillSO.TickDuration;
        AttackBaseData attackBase = skillSO.DamageStaggerPairs[0];
        EventManager.OnEnemyDataAcquired.Invoke(
            Physics2D.OverlapCircleAll(transform.position, skillSO.DamageRadius),
            new (skillSO.Element, transform.position, attackBase.Damage, attackBase.Stagger, attackBase.Knockback, attackBase.DebuffData)
        );

        if (Time.time < endTick) return;
        ActivateSkill(InputActionPhase.Canceled);
    }
    
    public override void ActivateSkill(InputActionPhase inputPhase)
    {
        switch (inputPhase)
        {
            case InputActionPhase.Started:
                if (isActive) break;
                isActive = true;
                movementScript.IncrementAcceleration(skillSO.AccelerationDelta);
                movementScript.IncrementDeceleration(skillSO.DecelerationDelta);
                movementScript.IncrementSpeed(skillSO.MaxSpeedDelta);
                movementScript.SetMovementDirection(MainCamera.MouseWorldPosition() - (Vector2)transform.position);
                nextTickTime = 0;
                endTick = Time.time + skillSO.MaxDuration;
                onActivate?.Invoke();
                break;
            case InputActionPhase.Canceled:
                if (!isActive) break;
                isActive = false;
                movementScript.IncrementAcceleration(-skillSO.AccelerationDelta);
                movementScript.IncrementDeceleration(-skillSO.DecelerationDelta);
                movementScript.IncrementSpeed(-skillSO.MaxSpeedDelta);
                onDeactivate?.Invoke();
                break;
        }
    }

    public override void Initialize(SkillTomeSO skillTomeSO, HotbarManager hotbarManager)
    {
        skillSO = skillTomeSO as MovementSkillSO;
        movementScript = hotbarManager.GetComponent<Movement>();
        isActive = false;
    }
}
