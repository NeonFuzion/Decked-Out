using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    private void Update()
    {

    }

    public void FireProjectile(ProjectileData projectileData, ProjectileTargetType projectileType, Vector3 targetPosition, int damage, int critChance)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Initialize(projectileData, targetPosition, damage, critChance);
    }
}
