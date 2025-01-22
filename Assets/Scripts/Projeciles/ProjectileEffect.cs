using UnityEngine;

[CreateAssetMenu]
public abstract class ProjectileEffect : ScriptableObject
{
    public abstract void ActivateEffect(Vector2 position);
}
