using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementSkillObject : SkillObject
{
    [SerializeField] UnityEvent onActivate, onDeactivate;

    MovementSkillSO skillSO;
    Movement movementScript;

    bool isActive;
    float nextTickTime;

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
    }
    
    public override void ActivateSkill(InputActionPhase inputPhase)
    {
        switch (inputPhase)
        {
            case InputActionPhase.Started:
                isActive = true;
                movementScript.IncrementAcceleration(skillSO.AccelerationDelta);
                movementScript.IncrementDeceleration(skillSO.DecelerationDelta);
                movementScript.IncrementDeceleration(skillSO.MaxSpeedDelta);
                nextTickTime = 0;
                onActivate?.Invoke();
                break;
            case InputActionPhase.Canceled:
                isActive = false;
                movementScript.IncrementAcceleration(-skillSO.AccelerationDelta);
                movementScript.IncrementDeceleration(-skillSO.DecelerationDelta);
                movementScript.IncrementDeceleration(-skillSO.MaxSpeedDelta);
                onDeactivate?.Invoke();
                break;
        }
    }

    public override void Initialize(SkillTomeSO skillTomeSO, HotbarManager hotbarManager)
    {
        skillSO = skillTomeSO as MovementSkillSO;
        movementScript = hotbarManager.GetComponent<Movement>();
    }
}
