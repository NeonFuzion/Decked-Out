using UnityEngine;

[CreateAssetMenu]
public class Projectile : ScriptableObject
{
    [SerializeField] float lifeSpan, speed;
    [SerializeField] int damage;
    [SerializeField] bool isPiercing;
    [SerializeField] Sprite sprite;
    [SerializeField] Vector2 hitboxSize;

    public float Lifespan { get => lifeSpan; }
    public float Speed { get => speed; }
    public int Damage { get => damage; }
    public bool IsPiercing { get => isPiercing; }
    public Sprite Sprite { get => sprite; }
    public Vector2 HitboxSize { get => hitboxSize; }

    public virtual void Movement(Rigidbody2D rigidbody)
    {
        rigidbody.linearVelocity = Vector2.right * speed;
    }
}
