using UnityEngine;

[CreateAssetMenu(menuName = "Item/Projectile/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] float speed, maxHeight, maxDistance, homingSpeed, damageRadius, rotationOffset;
    [SerializeField] Sprite sprite;
    [SerializeField] AnimationCurve trajectoryCurve;
    [SerializeField] ProjectileEffect projectileEffect;

    public float Speed { get => speed; }
    public float MaxHeight { get => maxHeight; }
    public float MaxDistance { get => maxDistance; }
    public float HomingSpeed { get => homingSpeed; }
    public float DamageRadius { get => damageRadius; }
    public float RotationOffset { get => rotationOffset; }
    public Sprite Sprite { get => sprite; }
    public AnimationCurve TrajectoryCurve { get => trajectoryCurve; }
    public ProjectileEffect ProjectileEffect { get => projectileEffect; }
}
