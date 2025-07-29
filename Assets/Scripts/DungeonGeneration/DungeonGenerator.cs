using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int roomLength;
    [SerializeField] float chestSpawnChance;
    [SerializeField] Tilemap wallTilemap, floorTilemap;
    [SerializeField] GameObject player, map, roomObjectParent, dungeonCreator;
    [SerializeField] GameObject[] enemies;
    [SerializeField] Item[] lootPool;
    [SerializeField] UnityEvent onRoomCleared;
    [SerializeField] GameObject[] roomTransitions;
    [SerializeField] DungeonRoomLayout[] layouts, specialLayouts;

    int enemyQuota, currentEnemyQuota;

    List<DungeonRoom> roomList;
    DungeonRoom currentRoom;

    public bool IsRoomCleared { get => currentRoom.IsSafe; set => currentRoom.IsSafe = value; }

    public Item[] LootPool { get => lootPool; }
    public GameObject[] EnemyPool { get => enemies; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonCreator.SetActive(false);
        DungeonRoom oldRoom = new(new Vector2(0, 0), new(), specialLayouts[0], this);
        roomList = new() { oldRoom };

        GeneratePath(oldRoom, Vector2.zero, 0, roomLength);

        while (roomObjectParent.transform.childCount > 0)
        {
            DestroyImmediate(roomObjectParent.transform.GetChild(0).gameObject);
        }

        foreach (DungeonRoom room in roomList)
        {
            GameObject roomImage = new GameObject();
            roomImage.transform.SetParent(map.transform);

            Image image = roomImage.AddComponent<Image>();
            image.rectTransform.localScale = new Vector2(0.3f, 0.3f);
            image.rectTransform.position = room.Position;

            foreach (Direction exit in room.Exits)
            {
                Vector2 linePosition = room.Position + DirectionToVector(exit) / 2;

                GameObject exitImage = new GameObject();
                exitImage.transform.SetParent(map.transform);

                Image line = exitImage.AddComponent<Image>();
                line.rectTransform.localScale = (int)exit % 2 == 0 ? new Vector2(0.05f, 0.25f) : new Vector2(0.25f, 0.05f);
                line.rectTransform.position = linePosition;
            }
        }

        currentRoom = roomList[0];
        LoadRoom(Direction.None);
    }

    // Update is called once per frame
    void Update()
    {

    }

    Direction VectorToDirection(Vector2 vector)
    {
        Direction direction = Direction.North;
        if (vector == Vector2.up) direction = Direction.North;
        else if (vector == Vector2.right) direction = Direction.East;
        else if (vector == Vector2.down) direction = Direction.South;
        else if (vector == Vector2.left) direction = Direction.West;
        else if (vector == Vector2.zero) direction = Direction.None;
        return direction;
    }

    Vector2 DirectionToVector(Direction direction)
    {
        Vector2 vector = Vector2.zero;
        switch ((int)direction)
        {
            case 0: vector = Vector2.up; break;
            case 1: vector = Vector2.right; break;
            case 2: vector = Vector2.down; break;
            case 3: vector = Vector2.left; break;
            case 4: vector = Vector2.zero; break;
        }
        return vector;
    }

    void PlaceTiles(List<TileInfo> tiles, Tilemap tilemap)
    {
        foreach (TileInfo tile in tiles)
        {
            tilemap.SetTile(tile.Position, tile.Tile);
        }
    }

    void GeneratePath(DungeonRoom oldRoom, Vector2 lastDirection, int currentPathLength, int pathLength)
    {
        List<Vector2> availableDirections = new() { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        if (availableDirections.Contains(-lastDirection)) availableDirections.Remove(-lastDirection);
        Vector2 direction = availableDirections[Random.Range(0, 3)];

        DungeonRoom newRoom;
        Vector2 newPostion = oldRoom.Position + direction;
        Direction newExit = VectorToDirection(-direction);
        DungeonRoom exitingRoom = FindRoomAtPosition(newPostion);
        if (exitingRoom != null)
        {
            exitingRoom.AddExit(newExit);
            newRoom = exitingRoom;
        }
        else
        {
            float specialRoomChance = Random.Range(0, 100);
            DungeonRoomLayout layout = specialRoomChance < 15 ? specialLayouts[Random.Range(1, specialLayouts.Length)] : layouts[Random.Range(0, layouts.Length)];
            newRoom = new (newPostion, new() { newExit }, layout, this);
            roomList.Add(newRoom);
        }
        oldRoom.AddExit(VectorToDirection(direction));

        if (currentPathLength >= pathLength) return;
        for (int i = 0; i < Random.Range(1, 3); i++)
            GeneratePath(newRoom, direction, currentPathLength + 1, pathLength);
    }

    public DungeonRoom FindRoomAtPosition(Vector2 position)
    {
        return roomList.Where(room => room.Position == position).FirstOrDefault();
    }

    public void LoadRoom(Direction direction)
    {
        currentRoom = FindRoomAtPosition(currentRoom.Position + DirectionToVector(direction));
        Vector3 directionVector = DirectionToVector(direction);
        for (int i = 0; i < map.transform.childCount; i++)
        {
            map.transform.GetChild(i).transform.position -= directionVector;
        }

        if (currentRoom == null) return;
        List<Direction> exits = currentRoom.Exits;
        DungeonRoomLayout roomLayout = currentRoom.DungeonRoomLayout;

        List<Vector2> transitionPositions = roomLayout.RoomTransitionPositions;
        for (int i = 0; i < transitionPositions.Count; i++)
        {
            GameObject transition = roomTransitions[i];
            transition.transform.position = transitionPositions[i];
            transition.GetComponent<RoomTransition>().ResetBarrier();
        }

        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        PlaceTiles(roomLayout.FloorTiles, floorTilemap);
        PlaceTiles(roomLayout.WallTiles, wallTilemap);
        if (!exits.Contains(Direction.North)) PlaceTiles(roomLayout.NorthExitTiles, wallTilemap);
        if (!exits.Contains(Direction.East)) PlaceTiles(roomLayout.EastExitTiles, wallTilemap);
        if (!exits.Contains(Direction.South)) PlaceTiles(roomLayout.SouthExitTiles, wallTilemap);
        if (!exits.Contains(Direction.West)) PlaceTiles(roomLayout.WestExitTiles, wallTilemap);

        switch (direction)
        {
            case Direction.North: roomTransitions[2].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.East: roomTransitions[3].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.South: roomTransitions[0].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.West: roomTransitions[1].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.None: roomTransitions[2].GetComponent<RoomTransition>().SpawnPlayer(player); break;
        }
        
        for (int i = 0; i < 4; i++)
        {
            bool containsExit = exits.Contains((Direction)i);
            roomTransitions[i].SetActive(containsExit);
        }

        for (int i = roomObjectParent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject roomObject = roomObjectParent.transform.GetChild(i).gameObject;
            Destroy(roomObject);
        }

        List<PrefabPositionPair> dataPairs = currentRoom.DungeonRoomLayout.RoomObjectPositions;
        for (int i = 0; i < dataPairs.Count; i++)
        {
            GameObject roomObject = Instantiate(dataPairs[i].Prefab, roomObjectParent.transform);
            roomObject.transform.position = dataPairs[i].Position;
            RoomObject script = roomObject.GetComponent<RoomObject>();
            script.LoadData(currentRoom.RoomObjects[i], this);
        }

        if (!currentRoom.IsSafe)
        {
            EventManager.InvokeOnCombatStarted();
        }
        else
        {
            foreach (GameObject roomTransition in roomTransitions)
            {
                roomTransition.GetComponent<RoomTransition>().UnlockBarrier();
            }
        }
    }

    public void IncrementCurrentEnemyQuota()
    {
        currentEnemyQuota++;

        if (currentEnemyQuota < enemyQuota) return;
        currentRoom.IsSafe = true;
        onRoomCleared?.Invoke();
        EventManager.InvokeOnCombatEnded();
    }

    public void IncrementEnemyQuota()
    {
        enemyQuota++;
    }
}

public enum Direction { North, East, South, West, None }

public class DungeonRoom
{
    bool isSafe;

    List<Direction> exits;
    List<RoomObjectData> roomObjects;
    Vector2 position;
    DungeonRoomLayout layout;

    public bool IsSafe { get => isSafe; set => isSafe = value; }

    public Vector2 Position { get => position; }
    public List<Direction> Exits { get => exits; }
    public List<RoomObjectData> RoomObjects { get => roomObjects; }

    public DungeonRoomLayout DungeonRoomLayout { get => layout; set => layout = value; }

    public DungeonRoom(Vector2 position, List<Direction> exits, DungeonRoomLayout layout, DungeonGenerator dungeonGenerator)
    {
        this.position = position;
        this.exits = exits;
        this.layout = layout;

        isSafe = true;

        roomObjects = layout.RoomObjectPositions.Select(x => x.Prefab.GetComponent<RoomObject>().Initialize(dungeonGenerator)).ToList();
    }

    public void AddExit(Direction direction)
    {
        if (exits.Contains(direction)) return;
        exits.Add(direction);
    }
}

[System.Serializable]
public class PrefabPositionPair
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector2 position;

    public GameObject Prefab { get => prefab; }
    public Vector2 Position { get => position; }

    public PrefabPositionPair(GameObject prefab, Vector2 position)
    {
        this.prefab = prefab;
        this.position = position;
    }
}