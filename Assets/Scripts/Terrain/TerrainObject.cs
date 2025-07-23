using UnityEngine;
using UnityEngine.Events;

public abstract class TerrainObject : MonoBehaviour
{
    public abstract RoomObjectData Initialize(DungeonGenerator dungeonGenerator);
    public abstract void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator);
}

public class RoomObjectData
{
    public bool IsActive;

    public RoomObjectData(bool isActive)
    {
        IsActive = isActive;
    }
}