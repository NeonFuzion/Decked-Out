using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/AngleBlast")]
public class AngleBlastSkillSO : SkillTomeSO
{
    [SerializeField] float range = 3f, coneAngle = 90f, knockbackForce = 5f;
    [SerializeField] LayerMask enemyLayer;

    public float Range => range;
    public float ConeAngle => coneAngle;
    public float KnockbackForce => knockbackForce;
    public LayerMask EnemyLayer => enemyLayer;
}
