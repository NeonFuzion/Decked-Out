using UnityEngine;
using UnityEngine.Events;

public abstract class TerrainObject : MonoBehaviour
{
    public abstract RoomObjectData Initialize();
    public abstract void LoadData(RoomObjectData roomObjectData);
}

public class RoomObjectData
{
    public bool IsActive;

    public RoomObjectData(bool isActive)
    {
        IsActive = isActive;
    }
}