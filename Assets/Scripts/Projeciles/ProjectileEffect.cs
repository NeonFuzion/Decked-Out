using UnityEngine;

public abstract class ProjectileEffect
{
    public abstract void ActivateEffect(Vector2 position, Collider2D[] colliders);
}
