using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform projectileVisual, projectileShadow;

    float totalDistance, groundDirection;
    int damage, critChance;

    ProjectileData projectileData;
    Vector2 targetPosition, startPosition;
    SpriteRenderer visualSpriteRenderer, shadowSpriteRenderer;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalDistance = Vector2.Distance(startPosition, targetPosition);
        transform.position += (Vector3)(targetPosition - startPosition).normalized * projectileData.Speed * Time.deltaTime;

        float distanceCovered = Vector2.Distance(transform.position, startPosition);
        float distanceProgress = distanceCovered / totalDistance;
        float trajectoryCurveValue = projectileData.TrajectoryCurve.Evaluate(distanceProgress);
        float projectileHeight = trajectoryCurveValue * projectileData.MaxHeight * totalDistance / 8;
        projectileVisual.transform.localPosition = Vector2.up * projectileHeight;

        Vector2 differenceVector = targetPosition - (Vector2)transform.position;
        float radians = Mathf.Atan2(differenceVector.y, differenceVector.x);
        groundDirection = (radians > 0 ? radians : radians + 2 * Mathf.PI) * 180 / Mathf.PI % 360;

        float trajectoryAngle = (1 - trajectoryCurveValue) * (distanceProgress > 0.5f && projectileData.MaxHeight != 0 ? -1 : 1) * projectileData.MaxHeight * 20;
        projectileVisual.transform.eulerAngles = Vector3.forward * (groundDirection + trajectoryAngle);
        projectileShadow.transform.eulerAngles = Vector3.forward * groundDirection;

        if (distanceProgress > 0.95f) DestroyProjectile();

        Collider2D collider = Physics2D.OverlapCircle(transform.position, projectileData.DamageRadius);
        if (!collider) return;
        if (collider.gameObject != gameObject) DestroyProjectile();
    }

    void DestroyProjectile()
    {
        if (projectileData.ProjectileEffect) projectileData.ProjectileEffect.ActivateEffect(targetPosition);
        Destroy(gameObject);
    }

    public void Initialize(ProjectileData projectileData, Vector2 targetPosition, int damage, int critChance)
    {
        this.projectileData = projectileData;
        this.damage = damage;
        this.critChance = critChance;

        Vector3 correctedTargetPosition = ((Vector3)targetPosition - transform.position).normalized * projectileData.MaxDistance + transform.position;
        this.targetPosition = projectileData.MaxHeight == 0 ? correctedTargetPosition : targetPosition;
        
        startPosition = transform.position;
        visualSpriteRenderer = projectileVisual.GetComponent<SpriteRenderer>();
        shadowSpriteRenderer = projectileShadow.GetComponent<SpriteRenderer>();
        visualSpriteRenderer.sprite = projectileData.Sprite;
        shadowSpriteRenderer.sprite = projectileData.Sprite;
    }
}

public enum ProjectileTargetType { Friendly, Enemy, Universal }