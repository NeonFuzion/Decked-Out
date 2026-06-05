using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] float mapSpacing, hallwayOffset;
    [SerializeField] Color playerColor;
    [SerializeField] RectTransform map;
    [SerializeField] Sprite hallwaySprite;

    DungeonRoom[] roomList;
    Dictionary<Vector2, Image> mapRooms;
    Vector2 currentPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapRooms = new ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetRoom(Vector2 position)
    {
        if (mapRooms.ContainsKey(currentPosition)) mapRooms[currentPosition].color = Color.white;
        currentPosition = position;
        mapRooms[currentPosition].gameObject.SetActive(true);
        mapRooms[currentPosition].color = playerColor;
    }

    public void OnRoomsCreated(DungeonRoom[] roomList)
    {
        this.roomList = roomList;
        Vector2 offset = new (roomList.Average(room => room.Position.x), roomList.Average(room => room.Position.y));
        
        foreach (DungeonRoom room in roomList)
        {
            GameObject roomImage = new GameObject(room.DungeonRoomLayout.name);
            roomImage.transform.SetParent(map);

            Image image = roomImage.AddComponent<Image>();
            image.sprite = room.DungeonRoomLayout.RoomIcon;
            image.SetNativeSize();
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.position = (room.Position - offset) * mapSpacing;
            image.gameObject.SetActive(false);
            mapRooms.Add(room.Position, image);

            foreach (Direction exit in room.Exits)
            {
                Vector2 linePosition = (room.Position + DungeonGenerator.DirectionToVector(exit) * hallwayOffset - offset) * mapSpacing;
                GameObject exitImage = new GameObject("Hallway");
                exitImage.transform.SetParent(image.transform);

                Image line = exitImage.AddComponent<Image>();
                line.sprite = hallwaySprite;
                line.SetNativeSize();
                line.rectTransform.localScale = Vector3.one;
                line.rectTransform.eulerAngles = new (0, 0, (int)exit % 2 == 0 ? 90 : 0);
                line.rectTransform.position = linePosition;
            }
        }

        SetRoom(roomList[0].Position);
    }

    public void ChangeRoom(Direction direction)
    {
        SetRoom(currentPosition + DungeonGenerator.DirectionToVector(direction));
    }
}
