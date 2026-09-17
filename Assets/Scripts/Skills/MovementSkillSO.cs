using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/Movement")]
public class MovementSkillSO : SkillTomeSO
{
    [field: SerializeField] public float MaxDuration { get; private set; }
    [field: SerializeField] public float AccelerationDelta { get; private set; }
    [field: SerializeField] public float DecelerationDelta { get; private set; }
    [field: SerializeField] public float MaxSpeedDelta { get; private set; }
    [field: SerializeField] public float TickDuration { get; private set; }
    [field: SerializeField] public float DamageRadius { get; private set; }
}
