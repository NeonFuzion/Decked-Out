using UnityEngine;

[CreateAssetMenu]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private float projectileMaxMoveSpeed;
    [SerializeField] private float projectileMaxHeight;
    [SerializeField] private float projectileMaxDistance;
    [SerializeField] private bool isHoming;
    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
    [SerializeField] private Sprite projectileSprite;

    public float ProjectileMaxMoveSpeed { get => projectileMaxMoveSpeed; }
    public float ProjectileMaxHeight { get => projectileMaxHeight; }
    public float ProjectileMaxDistance { get => projectileMaxDistance; }
    public bool IsHoming { get => isHoming; }
    public AnimationCurve TrajectoryAnimationCurve { get => trajectoryAnimationCurve; }
    public AnimationCurve AxisCorrectionAnimationCurve { get => axisCorrectionAnimationCurve; }
    public AnimationCurve ProjectileSpeedAnimationCurve { get => projectileSpeedAnimationCurve; }
    public Sprite ProjectileSprite { get => projectileSprite; }
}
