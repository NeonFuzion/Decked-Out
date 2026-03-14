using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform projectileVisual, projectileShadow;

    float totalDistance, groundDirection;

    ProjectileData projectileData;
    ProjectileEffect projectileEffect;
    Vector2 targetPosition, startPosition;
    SpriteRenderer visualSpriteRenderer, shadowSpriteRenderer;
    UnityEvent<Collider2D[], Projectile> onHit;
    UnityEvent<Projectile> onExpire;
    
    public UnityEvent<Collider2D[], Projectile> OnHit { get => onHit; }
    public UnityEvent<Projectile> OnExpire { get => onExpire; }
    public ProjectileData ProjectileData { get => projectileData; }
    public ProjectileEffect ProjectileEffect { get => projectileEffect; }

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

        float trajectoryAngle = (1 - trajectoryCurveValue) * (distanceProgress > 0.5f && projectileData.MaxHeight != 0 ? -1 : 1) * projectileData.MaxHeight * 20 + projectileData.RotationOffset;
        projectileVisual.transform.eulerAngles = Vector3.forward * (groundDirection + trajectoryAngle);
        projectileShadow.transform.eulerAngles = Vector3.forward * groundDirection;

        if (distanceProgress <= 1 && projectileData.MaxHeight > 0) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, projectileData.DamageRadius).Where(collider => collider.gameObject != gameObject).ToArray();

        if (projectileData.MaxHeight > 0)
        {
            if (colliders.Length > 0) onHit?.Invoke(colliders, this);
            Destroy(gameObject);
        }
        else
        {
            if (distanceProgress < 1)
            {
                if (colliders.Length == 0) return;
                onHit?.Invoke(colliders, this);
            }
            else
            {
                onExpire?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(ProjectileData projectileData, Vector2 targetPosition)
    {
        this.projectileData = projectileData;

        Vector3 correctedTargetPosition = ((Vector3)targetPosition - transform.position).normalized * projectileData.MaxDistance + transform.position;
        this.targetPosition = projectileData.MaxHeight == 0 ? correctedTargetPosition : targetPosition;
        
        onHit = new ();
        startPosition = transform.position;
        visualSpriteRenderer = projectileVisual.GetComponent<SpriteRenderer>();
        shadowSpriteRenderer = projectileShadow.GetComponent<SpriteRenderer>();
        visualSpriteRenderer.sprite = projectileData.Sprite;
        shadowSpriteRenderer.sprite = projectileData.Sprite;

        if (!projectileData.ProjectileEffect) return;
        projectileEffect = Instantiate(projectileData.ProjectileEffect);
    }
}