using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    [SerializeField] float radialFireRadius;
    [SerializeField] BeingType senderType;
    [SerializeField] Transform firePoint;

    public Transform FirePoint { get => firePoint; }

    private void Update()
    {

    }

    public void FireProjectile(GameObject projectilePrefab, Vector3 targetPosition, out Projectile projectile, FiringMode firingMode)
    {
        Vector2 radialFirePosition = (targetPosition - transform.position).normalized * radialFireRadius + transform.position;
        Vector2 startPosition = firingMode == FiringMode.FirePoint ? firePoint.position : radialFirePosition;
        projectile = Instantiate(projectilePrefab, startPosition, Quaternion.identity).GetComponent<Projectile>();
        projectile.Initialize(targetPosition);
    }
}

public enum FiringMode { None, FirePoint, Radial }
