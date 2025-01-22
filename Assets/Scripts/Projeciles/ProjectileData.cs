using UnityEngine;

[CreateAssetMenu]
public class ProjectileData : ScriptableObject
{
    [SerializeField] float speed, maxHeight, maxDistance, homingSpeed, damageRadius;
    [SerializeField] Sprite sprite;
    [SerializeField] AnimationCurve trajectoryCurve;
    [SerializeField] ProjectileEffect projectileEffect;

    public float Speed { get => speed; }
    public float MaxHeight { get => maxHeight; }
    public float MaxDistance { get => maxDistance; }
    public float HomingSpeed { get => homingSpeed; }
    public float DamageRadius { get => damageRadius; }
    public Sprite Sprite { get => sprite; }
    public AnimationCurve TrajectoryCurve { get => trajectoryCurve; }
    public ProjectileEffect ProjectileEffect { get => projectileEffect; }
}
