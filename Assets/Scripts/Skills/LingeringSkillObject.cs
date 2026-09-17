using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LingeringSkillObject : SkillObject
{
    LingeringSkillSO skillSO;
    ParticleSystem particleSystem;
    Transform skillParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize(SkillTomeSO skillTomeSO, HotbarManager hotbarManager)
    {
        particleSystem = GetComponent<ParticleSystem>();
        skillSO = skillTomeSO as LingeringSkillSO;
        skillParent = transform.parent;
    }
    
    public override void ActivateSkill(InputActionPhase inputPhase)
    {
        if (inputPhase != InputActionPhase.Started) return;
        Vector2 mousePos = MainCamera.MouseWorldPosition();
        Vector2 direction = (mousePos - (Vector2)skillParent.position).normalized;
        Vector2 spawnPos = (Vector2)skillParent.position + direction * skillSO.SpawnDistance;

        GameObject obj = particleSystem.gameObject;
        obj.transform.SetParent(null);
        obj.transform.position = spawnPos;
        particleSystem.Play();

        Timer timer = obj.GetComponent<Timer>();
        timer.SetTimer(skillSO.TickInterval);

        MultiTrigger multiTrigger = obj.GetComponent<MultiTrigger>();
        multiTrigger.Initialize(skillSO.TickCount);
        multiTrigger.OnTrigger.AddListener(() => {
            AttackBaseData attackBase = skillSO.DamageStaggerPairs[0];
            EventManager.OnEnemyDataAcquired.Invoke(
                Physics2D.OverlapCircleAll(obj.transform.position, skillSO.Radius),
                new (skillSO.Element, obj.transform.position, attackBase.Damage, attackBase.Stagger, attackBase.Knockback, attackBase.DebuffData)
            );
        });

        StartCoroutine(VisualLagCoroutine(particleSystem, obj.transform));
    }

    IEnumerator VisualLagCoroutine(ParticleSystem particleSystem, Transform visual)
    {
        yield return new WaitWhile(() => particleSystem.isPlaying);
        visual.SetParent(skillParent);
    }
}
