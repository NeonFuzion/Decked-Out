using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/AngleBlast")]
public class AngleBlastSkillSO : SkillTomeSO
{
    [SerializeField] float range = 3f, coneAngle = 90f, knockbackForce = 5f;
    [SerializeField] LayerMask enemyLayer;

    public override void ActivateEffects(HotbarManager hotbarManager, int index)
    {
        Vector2 playerPos = hotbarManager.transform.position;
        Vector2 direction = (MainCamera.MouseWorldPosition() - playerPos).normalized;

        ParticleSystem particleSystem = hotbarManager.GetParticleSystem(index);
        particleSystem.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - coneAngle / 2);
        particleSystem.Play();

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, range, enemyLayer);

        IEnumerable<Collider2D> inCone = hits.Where(hit => {
            Vector2 toEnemy = ((Vector2)hit.transform.position - playerPos).normalized;
            return Vector2.Angle(direction, toEnemy) <= coneAngle / 2f;
        });

        if (inCone.Count() == 0) return;
        AttackBaseData pair = DamageStaggerPairs[0];
        AttackData attackData = new(Element, playerPos, pair.Damage, pair.Stagger, (int)knockbackForce, pair.IsDebuffing ? DebuffData : new ());
        EventManager.OnEnemyDataAcquired.Invoke(inCone.ToArray(), attackData);
    }
}
