using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileVisual projectileVisual;

    private Transform target;
    private bool isHoming;
    private float moveSpeed;
    private float maxMoveSpeed;
    private float trajectoryMaxRelativeHeight;
    private float distanceToTargetToDestroyProjectile = 1f;

    private AnimationCurve trajectoryAnimationCurve;
    private AnimationCurve axisCorrectionAnimationCurve;
    private AnimationCurve projectileSpeedAnimationCurve;

    private Vector3 trajectoryStartPoint;
    private Vector3 projectileMoveDir;
    private Vector3 trajectoryRange;
    private Vector3 targetPosition;

    private float nextYTrajectoryPosition;
    private float nextXTrajectoryPosition;
    private float nextPositionYCorrectionAbsolute;
    private float nextPositionXCorrectionAbsolute;

    private void Start() {
        trajectoryStartPoint = transform.position;
    }

    private void Update() {
        if (isHoming) targetPosition = target.position;

        UpdateProjectilePosition();

        if (Vector3.Distance(transform.position, targetPosition) >= distanceToTargetToDestroyProjectile) return;
        Destroy(gameObject);
    }

    private void UpdateProjectilePosition() {
        trajectoryRange = targetPosition - trajectoryStartPoint;

        if(Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y)) {
            // Projectile will be curved on the X axis

            if (trajectoryRange.y < 0) {
                // Target is located under shooter
                moveSpeed = -moveSpeed;
            }

            UpdatePositionWithXCurve();

        } else {
            // Projectile will be curved on the Y axis

            if (trajectoryRange.x < 0) {
                // Target is located behind shooter
                moveSpeed = -moveSpeed;
            }

            UpdatePositionWithYCurve();
        }
    }

    private void UpdatePositionWithXCurve() {

        float nextPositionY = transform.position.y + moveSpeed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - trajectoryStartPoint.y) / trajectoryRange.y;

        float nextPositionXNormalized = trajectoryAnimationCurve.Evaluate(nextPositionYNormalized);
        nextXTrajectoryPosition = nextPositionXNormalized * trajectoryMaxRelativeHeight;

        float nextPositionXCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionYNormalized);
        nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * trajectoryRange.x;

        if ((trajectoryRange.x > 0 && trajectoryRange.y > 0) || (trajectoryRange.x < 0 && trajectoryRange.y < 0)) {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }

        float nextPositionX = trajectoryStartPoint.x + nextXTrajectoryPosition + nextPositionXCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionYNormalized);
        projectileMoveDir = newPosition - transform.position;

        transform.position = newPosition;
    }

    private void UpdatePositionWithYCurve() {
        float nextPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - trajectoryStartPoint.x) / trajectoryRange.x;

        float nextPositionYNormalized = trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
        nextYTrajectoryPosition = nextPositionYNormalized * trajectoryMaxRelativeHeight;

        float nextPositionYCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
        nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;

        float nextPositionY = trajectoryStartPoint.y + nextYTrajectoryPosition + nextPositionYCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionXNormalized);
        projectileMoveDir = newPosition - transform.position;

        transform.position = newPosition;
    }

    private void CalculateNextProjectileSpeed(float nextPositionXNormalized) {
        float nextMoveSpeedNormalized = projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);

        moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
    }

    public void InitializeProjectile(Vector3 targetPosition, bool isHoming, float maxMoveSpeed, float trajectoryMaxHeight, Transform target) {
        if (target) this.target = target;
        else this.targetPosition = targetPosition;

        this.isHoming = isHoming;
        this.maxMoveSpeed = maxMoveSpeed;

        float xDistanceToTarget = targetPosition.x - transform.position.x;
        trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;

        projectileVisual.SetTarget(target, isHoming);
    }

    public void InitializeAnimationCurves(AnimationCurve trajectoryAnimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve) {
        this.trajectoryAnimationCurve = trajectoryAnimationCurve;
        this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
        this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
    }

    public void InitializeProjectileSprites(Sprite projectileSprite)
    {
        projectileVisual.Initialize(projectileSprite);
    }

    public Vector3 GetProjectileMoveDir() {
        return projectileMoveDir;
    }

    public float GetNextYTrajectoryPosition() {
        return nextYTrajectoryPosition;
    }

    public float GetNextPositionYCorrectionAbsolute() {
        return nextPositionYCorrectionAbsolute;
    }

    public float GetNextXTrajectoryPosition() {
        return nextXTrajectoryPosition;
    }

    public float GetNextPositionXCorrectionAbsolute() {
        return nextPositionXCorrectionAbsolute;
    }
}
