using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform projectileVisual, projectileShadow;
    [SerializeField] float speed, maxHeight, maxDistance, homingSpeed, damageRadius;
    [SerializeField] AnimationCurve trajectoryCurve;
    [SerializeField] ProjectileEffect projectileEffect;

    float totalDistance, groundDirection, rotationOffset;

    Vector2 targetPosition, startPosition;
    SpriteRenderer visualSpriteRenderer, shadowSpriteRenderer;
    UnityEvent<Collider2D[], Projectile> onHit;
    UnityEvent<Projectile> onExpire;
    
    public UnityEvent<Collider2D[], Projectile> OnHit { get => onHit; }
    public UnityEvent<Projectile> OnExpire { get => onExpire; }
    public ProjectileEffect ProjectileEffect { get => projectileEffect; }
    public float MaxHeight { get => maxHeight; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalDistance = Vector2.Distance(startPosition, targetPosition);
        transform.position += (Vector3)(targetPosition - startPosition).normalized * speed * Time.deltaTime;

        float distanceCovered = Vector2.Distance(transform.position, startPosition);
        float distanceProgress = distanceCovered / totalDistance;
        float trajectoryCurveValue = trajectoryCurve.Evaluate(distanceProgress);
        float projectileHeight = trajectoryCurveValue * maxHeight * totalDistance / 8;
        projectileVisual.transform.localPosition = Vector2.up * projectileHeight;

        Vector2 differenceVector = targetPosition - (Vector2)transform.position;
        float radians = Mathf.Atan2(differenceVector.y, differenceVector.x);
        groundDirection = (radians > 0 ? radians : radians + 2 * Mathf.PI) * 180 / Mathf.PI % 360;

        float trajectoryAngle = (1 - trajectoryCurveValue) * (distanceProgress > 0.5f && maxHeight != 0 ? -1 : 1) * maxHeight * 20 + rotationOffset;
        projectileVisual.transform.eulerAngles = Vector3.forward * (groundDirection + trajectoryAngle);
        projectileShadow.transform.eulerAngles = Vector3.forward * groundDirection;

        if (distanceProgress <= 1 && maxHeight > 0) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRadius).Where(collider => collider.gameObject != gameObject).ToArray();

        if (maxHeight > 0)
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

    public void Initialize(Vector2 targetPosition)
    {
        Vector3 correctedTargetPosition = ((Vector3)targetPosition - transform.position).normalized * maxDistance + transform.position;
        this.targetPosition = maxHeight == 0 ? correctedTargetPosition : targetPosition;
        
        onHit = new ();
        rotationOffset = projectileVisual.eulerAngles.z;
        startPosition = transform.position;
        visualSpriteRenderer = projectileVisual.GetComponent<SpriteRenderer>();
        shadowSpriteRenderer = projectileShadow.GetComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = visualSpriteRenderer.sprite;
    }
}