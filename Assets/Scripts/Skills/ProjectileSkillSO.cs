using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/Projectile")]
public class ProjectileSkillSO : SkillTomeSO
{
    [SerializeField] GameObject prefabProjectile;

    public GameObject PrefabProjectile => prefabProjectile;
}