using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AngleBlastSkillObject : SkillObject
{
    ParticleSystem particleSystem;
    AngleBlastSkillSO skillSO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize(SkillTomeSO skillTomeSO)
    {
        particleSystem = GetComponent<ParticleSystem>();
        skillSO = skillTomeSO as AngleBlastSkillSO;
    }
    
    public override void ActivateSkill()
    {
        Vector2 playerPos = transform.position;
        Vector2 direction = (MainCamera.MouseWorldPosition() - playerPos).normalized;

        particleSystem.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - skillSO.ConeAngle / 2);
        particleSystem.Play();

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, skillSO.Range, skillSO.EnemyLayer);

        IEnumerable<Collider2D> inCone = hits.Where(hit => {
            Vector2 toEnemy = ((Vector2)hit.transform.position - playerPos).normalized;
            return Vector2.Angle(direction, toEnemy) <= skillSO.ConeAngle / 2f;
        });

        if (inCone.Count() == 0) return;
        AttackBaseData pair = skillSO.DamageStaggerPairs[0];
        AttackData attackData = new(skillSO.Element, playerPos, pair.Damage, pair.Stagger, (int)skillSO.KnockbackForce, pair.IsDebuffing ? skillSO.DebuffData : new ());
        EventManager.OnEnemyDataAcquired.Invoke(inCone.ToArray(), attackData);
    }
}
