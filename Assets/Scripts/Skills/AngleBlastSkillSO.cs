using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/AngleBlast")]
public class AngleBlastSkillSO : SkillTomeSO
{
    [SerializeField] float range = 3f, coneAngle = 90f, knockbackForce = 5f;
    [SerializeField] Material particleMaterial;
    [SerializeField] LayerMask enemyLayer;

    public override void ActivateEffects(Player player, int index)
    {
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (MainCamera.MouseWorldPosition() - playerPos).normalized;

        player.FireParticles(direction, coneAngle, particleMaterial);

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, range, enemyLayer);

        List<Collider2D> inCone = hits.Where(hit => {
            Vector2 toEnemy = ((Vector2)hit.transform.position - playerPos).normalized;
            return Vector2.Angle(direction, toEnemy) <= coneAngle / 2f;
        }).ToList();

        if (inCone.Count == 0) return;
        DamageStaggerPair pair = DamageStaggerPairs[0];
        AttackData attackData = new(Element, playerPos, pair.Damage, pair.Stagger, (int)knockbackForce);
        EventManager.InvokeOnEnemyDataAcquired(inCone.ToArray(), attackData);
    }
}
