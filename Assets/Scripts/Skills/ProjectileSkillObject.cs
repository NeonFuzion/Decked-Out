using UnityEngine;
using UnityEngine.InputSystem;

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

    public override void Initialize(SkillTomeSO skillTomeSO, HotbarManager hotbarManager)
    {
        skillSO = skillTomeSO as ProjectileSkillSO;
        shooter = GetComponent<Shooter>();
    }

    public override void ActivateSkill(InputActionPhase inputPhase)
    {
        if (inputPhase != InputActionPhase.Started) return;
        Vector3 mousePosition = MainCamera.MouseWorldPosition();
        Vector3 direction = (mousePosition - transform.position).normalized;
        shooter.transform.eulerAngles = Vector3.forward * Mathf.Atan2(direction.y, direction.x);

        Projectile projectile;
        shooter.FireProjectile(skillSO.PrefabProjectile, mousePosition, out projectile, FiringMode.Radial);
        projectile.OnHit.AddListener((Collider2D[] colliders, Projectile projectile) =>
        {
            AttackBaseData attackBase = skillSO.DamageStaggerPairs[0];
            AttackData attackData = new (Element.Ice, projectile.transform.position, attackBase.Damage, attackBase.Stagger, attackBase.Knockback, attackBase.DebuffData);
            EventManager.OnEnemyDataAcquired.Invoke(colliders, attackData);
        });
    }
}
