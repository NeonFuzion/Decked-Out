using UnityEngine;
using System.Linq;
using System.Collections;

[CreateAssetMenu(menuName = "SkillTome/Lingering")]
public class LingeringSkillSO : SkillTomeSO
{
    [SerializeField] float spawnDistance = 2f, radius = 2.5f, tickInterval = 0.5f;
    [SerializeField] int knockBack = -5, tickCount = 6;

    public float SpawnDistance => spawnDistance;
    public float Radius => radius;
    public float TickInterval => tickInterval;
    public int Knockback => knockBack;
    public int TickCount => tickCount;
}
