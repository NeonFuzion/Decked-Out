using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int roomLength;
    [SerializeField] GameObject[] enemies;
    [SerializeField] Item[] lootPool;
    [SerializeField] DungeonRoomLayout[] layouts, specialLayouts;
    [SerializeField] UnityEvent onRoomCleared;
    [SerializeField] Tilemap wallTilemap, floorTilemap;
    [SerializeField] Tilemap[] exitTilemaps;
    [SerializeField] GameObject[] roomTransitions, enemySpawners;
    [SerializeField] GameObject prefabChest, player, map;

    int enemyQuota, currentEnemyQuota;

    List<DungeonRoom> roomList;
    DungeonRoom currentRoom;
    GameObject existingChest;

    public bool IsRoomCleared { get => currentRoom.IsRoomCleared; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonRoom oldRoom = new DungeonRoom(new Vector2(0, 0), new List<Direction>(), layouts[Random.Range(0, layouts.Length)]);
        roomList = new List<DungeonRoom>() { oldRoom };
        /*for (int i = 0; i < roomLength; i++)
        {
            Vector2 direction = Vector2.zero;
            while (true)
            {
                switch (Random.Range(0, 4))
                {
                    case 0: direction = Vector2.up; break;
                    case 1: direction = Vector2.right; break;
                    case 2: direction = Vector2.down; break;
                    case 3: direction = Vector2.left; break;
                }
                if (-direction != lastDirection) break;
            }
            lastDirection = direction;

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
                newRoom = new DungeonRoom(newPostion, new List<Direction>() { newExit }, layouts[Random.Range(0, layouts.Length)]);
                roomList.Add(newRoom);
            }
            oldRoom.AddExit(VectorToDirection(direction));
            oldRoom = newRoom;
        }*/

        GeneratePath(oldRoom, Vector2.zero, 0, roomLength);

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
        tilemap.ClearAllTiles();
        foreach (TileInfo tile in tiles)
        {
            tilemap.SetTile(tile.Position, tile.Tile);
            tilemap.SetTransformMatrix(tile.Position, Matrix4x4.Rotate(tile.Rotation));
        }
    }

    void SetPositions(List<Vector2> positions, GameObject[] gameObjects)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            gameObjects[i].transform.position = positions[i];
        }
    }

    void GeneratePath(DungeonRoom oldRoom, Vector2 lastDirection, int currentPathLength, int pathLength)
    {
        DungeonRoom newRoom = null;
        Vector2 direction = Vector2.zero;
        while (true)
        {
            switch (Random.Range(0, 4))
            {
                case 0: direction = Vector2.up; break;
                case 1: direction = Vector2.right; break;
                case 2: direction = Vector2.down; break;
                case 3: direction = Vector2.left; break;
            }
            if (-direction != lastDirection) break;
        }

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
            newRoom = new DungeonRoom(newPostion, new List<Direction>() { newExit }, layouts[Random.Range(0, layouts.Length)]);
            roomList.Add(newRoom);
        }
        oldRoom.AddExit(VectorToDirection(direction));

        if (currentPathLength >= pathLength) return;
        for (int i = 0; i < Random.Range(1, 3); i++)
            GeneratePath(newRoom, direction, currentPathLength + 1, pathLength);
    }

    public DungeonRoom FindRoomAtPosition(Vector2 position)
    {
        foreach (DungeonRoom room in roomList)
        {
            if (room.Position != position) continue;
            return room;
        }
        return null;
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

        SetPositions(roomLayout.EnemySpawnPositions, enemySpawners);
        SetPositions(roomLayout.RoomTransitionPositions, roomTransitions);

        PlaceTiles(roomLayout.FloorTiles, floorTilemap);
        PlaceTiles(roomLayout.WallTiles, wallTilemap);
        PlaceTiles(roomLayout.NorthExitTiles, exitTilemaps[0]);
        PlaceTiles(roomLayout.EastExitTiles, exitTilemaps[1]);
        PlaceTiles(roomLayout.SouthExitTiles, exitTilemaps[2]);
        PlaceTiles(roomLayout.WestExitTiles, exitTilemaps[3]);

        switch (direction)
        {
            case Direction.North: roomTransitions[2].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.East: roomTransitions[3].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.South: roomTransitions[0].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.West: roomTransitions[1].GetComponent<RoomTransition>().SpawnPlayer(player); break;
            case Direction.None: roomTransitions[2].GetComponent<RoomTransition>().SpawnPlayer(player); break;
        }

        if (currentRoom.IsChestOpened)
        {
            if (existingChest != null)
            {
                Destroy(existingChest);
                existingChest = null;
            }
        }

        if (exits == null || exits.Count == 0) return;
        for (int i = 0; i < exitTilemaps.Length; i++)
        {
            if (!exits.Contains((Direction)i))
            {
                exitTilemaps[i].gameObject.SetActive(true);
                roomTransitions[i].SetActive(false);
            }
            else
            {
                exitTilemaps[i].gameObject.SetActive(false);
                roomTransitions[i].SetActive(true);
            }
        }

        if (!currentRoom.IsRoomCleared) return;
        foreach (GameObject roomTransition in roomTransitions)
        {
            roomTransition.GetComponent<RoomTransition>().UnlockBarrier();
        }
    }

    public void SpawnEnemies()
    {
        if (currentRoom.IsRoomCleared) return;
        currentEnemyQuota = 0;
        enemyQuota = 0;
        foreach (GameObject enemySpawner in enemySpawners)
        {
            enemySpawner.GetComponent<EnemySpawner>().SpawnEnemy(enemies[Random.Range(0, enemies.Length)]);
            enemyQuota++;
        }
    }

    public void IncrementEnemyQuota()
    {
        currentEnemyQuota++;

        if (currentEnemyQuota < enemyQuota) return;
        currentRoom.IsRoomCleared = true;
        GameObject chest = Instantiate(prefabChest, currentRoom.DungeonRoomLayout.ChestPosition, Quaternion.identity);
        existingChest = chest;
        chest.GetComponent<LootDrops>().SingleDrops = lootPool.ToList();
        onRoomCleared?.Invoke();
    }

    public void SetChestOpened()
    {
        currentRoom.IsChestOpened = true;
        existingChest = null;
    }
}

public enum Direction { North, East, South, West, None }

public class DungeonRoom
{
    bool isRoomCleared, isChestOpened;

    Vector2 position;
    List<Direction> exits;
    DungeonRoomLayout layout;

    public bool IsRoomCleared { get => isRoomCleared; set => isRoomCleared = value; }
    public bool IsChestOpened { get => isChestOpened; set => isChestOpened = value; }

    public Vector2 Position { get => position; }
    public List<Direction> Exits { get => exits; }
    public DungeonRoomLayout DungeonRoomLayout { get => layout; }

    public DungeonRoom(Vector2 position, List<Direction> exits, DungeonRoomLayout layout)
    {
        this.position = position;
        this.exits = exits;
        this.layout = layout;

        isRoomCleared = false;
        isChestOpened = false;
    }

    public void AddExit(Direction direction)
    {
        if (exits.Contains(direction)) return;
        exits.Add(direction);
    }
}

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