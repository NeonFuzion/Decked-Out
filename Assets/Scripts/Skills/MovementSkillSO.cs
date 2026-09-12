using UnityEditor;
using UnityEngine;

public class MovementSkillSO : SkillTomeSO
{
    [field: SerializeField] public float MaxDuration { get; }
    [field: SerializeField] public float AccelerationDelta { get; }
    [field: SerializeField] public float DecelerationDelta { get; }
    [field: SerializeField] public float MaxSpeedDelta { get; }
    [field: SerializeField] public float TickDuration { get; }
    [field: SerializeField] public float DamageRadius { get; }
}
