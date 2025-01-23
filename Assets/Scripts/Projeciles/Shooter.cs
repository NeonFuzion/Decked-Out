using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    private void Update()
    {

    }

    public void FireProjectile(ProjectileData projectileData, ProjectileTargetType projectileType, Vector3 targetPosition, int damage, bool isCrit, UnityAction killReaction)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Initialize(projectileData, targetPosition, damage, isCrit, killReaction);
    }
}
