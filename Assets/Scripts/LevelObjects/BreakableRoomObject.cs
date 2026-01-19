using UnityEngine;

public class BreakableRoomObject : RoomObject
{
    RoomObjectData roomObjectData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBroken()
    {
        roomObjectData.IsActive = false;
        gameObject.SetActive(false);
    }
    
    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        return new (true);
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        this.roomObjectData = roomObjectData;
        gameObject.SetActive(this.roomObjectData.IsActive);
    }
}