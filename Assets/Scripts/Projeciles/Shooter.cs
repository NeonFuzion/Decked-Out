using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    private void Update()
    {

    }

    public void FireProjectile(ProjectileData projectileData, Vector3 direction = new Vector3(), Transform target = null)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();

        projectile.InitializeProjectile(direction * projectileData.ProjectileMaxDistance, projectileData.IsHoming, projectileData.ProjectileMaxMoveSpeed, projectileData.ProjectileMaxHeight, target);
        projectile.InitializeAnimationCurves(projectileData.TrajectoryAnimationCurve, projectileData.AxisCorrectionAnimationCurve, projectileData.ProjectileSpeedAnimationCurve);
        projectile.InitializeProjectileSprites(projectileData.ProjectileSprite);
    }
}
