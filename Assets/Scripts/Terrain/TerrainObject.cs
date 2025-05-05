using UnityEngine;
using UnityEngine.Events;

public abstract class TerrainObject : MonoBehaviour
{
    public abstract void Initialize(bool isActive, UnityAction<GameObject> action);
}
