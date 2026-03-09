using UnityEngine;

public abstract class ProjectileEffect : ScriptableObject
{
    public abstract void ActivateEffect(Vector2 position, Collider2D[] colliders);
}
