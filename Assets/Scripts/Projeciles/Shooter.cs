using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    public Transform FirePoint { get => firePoint; }

    private void Update()
    {

    }

    public void FireProjectile(ProjectileData projectileData, ProjectileTargetType projectileType, Vector3 targetPosition, Element element, int damage, bool isCrit)
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Initialize(projectileData, targetPosition, element, damage, isCrit);
    }
}
