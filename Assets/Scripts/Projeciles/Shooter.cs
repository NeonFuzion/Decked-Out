using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    [SerializeField] BeingType senderType;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    public Transform FirePoint { get => firePoint; }

    private void Update()
    {

    }

    public void FireProjectile(ProjectileSO projectileData, Vector3 targetPosition, out Projectile projectile)
    {
        projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Initialize(projectileData, targetPosition);
    }
}
