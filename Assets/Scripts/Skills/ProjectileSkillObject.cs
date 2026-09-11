using UnityEngine;

public class ProjectileSkillObject : SkillObject
{
    ProjectileSkillSO skillSO;
    Shooter shooter;

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
        skillSO = skillTomeSO as ProjectileSkillSO;
    }

    public override void ActivateSkill()
    {
        Projectile projectile;
        shooter.FireProjectile(skillSO.PrefabProjectile, MainCamera.MouseWorldPosition(), out projectile, FiringMode.Radial);
        projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) =>
        {
            AttackBaseData damageStaggerPair = skillSO.DamageStaggerPairs[0];
            AttackData attackData = new (Element.Ice, projectile.transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger, 1, damageStaggerPair.IsDebuffing ? skillSO.DebuffData : new ());
            EventManager.OnEnemyDataAcquired.Invoke(colliders, attackData);
        });
    }
}
