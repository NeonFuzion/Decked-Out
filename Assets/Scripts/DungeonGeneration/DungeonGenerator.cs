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
    [SerializeField] GameObject[] enemies;
    [SerializeField] Item[] lootPool;
    [SerializeField] UnityEvent onRoomCleared;
    [SerializeField] Tilemap wallTilemap, floorTilemap;
    [SerializeField] Tilemap[] exitTilemaps;
    [SerializeField] GameObject[] roomTransitions;
    [SerializeField] DungeonRoomLayout[] layouts, specialLayouts;
    [SerializeField] GameObject prefabChest, prefabEnemySpawner, player, map, specialObjectsParent;

    int enemyQuota, currentEnemyQuota;

    List<DungeonRoom> roomList;
    List<GameObject> enemySpawners, currentSpecialObjects;
    DungeonRoom currentRoom;
    GameObject existingChest;

    public bool IsRoomCleared { get => currentRoom.IsRoomCleared; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonRoom oldRoom = new DungeonRoom(new Vector2(0, 0), new (), specialLayouts[0]);
        roomList = new() { oldRoom };
        enemySpawners = new ();
        currentSpecialObjects = new ();
        
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

    void GeneratePath(DungeonRoom oldRoom, Vector2 lastDirection, int currentPathLength, int pathLength)
    {
        List<Vector2> availableDirections = new () { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
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
            newRoom = new DungeonRoom(newPostion, new () { newExit }, layouts[Random.Range(0, layouts.Length)]);
            roomList.Add(newRoom);
        }
        oldRoom.AddExit(VectorToDirection(direction));

        if (currentPathLength >= pathLength) return;
        for (int i = 0; i < Random.Range(1, 3); i++)
            GeneratePath(newRoom, direction, currentPathLength + 1, pathLength);
    }

    void SpawnChest()
    {
        GameObject chest = Instantiate(prefabChest, currentRoom.DungeonRoomLayout.ChestPosition, Quaternion.identity);
        existingChest = chest;
        chest.GetComponent<LootDrops>().SingleDrops = lootPool.ToList();
        chest.GetComponent<TreasureChest>().OnOpen.AddListener(SetChestOpened);
    }

    public DungeonRoom FindRoomAtPosition(Vector2 position)
    {
        return roomList.Where(room => room.Position == position).FirstOrDefault();
    }

    public void LoadRoom(Direction direction)
    {
        currentSpecialObjects.Clear();
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

        List<Vector2> enemyPositions = roomLayout.EnemySpawnPositions;
        while (enemyPositions.Count != enemySpawners.Count)
        {
            if (enemyPositions.Count > enemySpawners.Count)
            {
                GameObject spawner = Instantiate(prefabEnemySpawner, transform);
                spawner.GetComponent<EnemySpawner>().Initialize();
                spawner.transform.position = enemyPositions[enemySpawners.Count != 0 ? enemySpawners.Count : 0];
                enemySpawners.Add(spawner);
            }
            else
            {
                int index = enemySpawners.Count - 1;
                Destroy(enemySpawners[index]);
                enemySpawners.RemoveAt(index);
            }
        }
        for (int i = 0; i < enemyPositions.Count; i++)
        {
            if (enemySpawners.Count < i) continue;
            enemySpawners[i].transform.position = enemyPositions[i];
        }

        foreach (PrefabPositionPair pair in roomLayout.SpecialObjectPositions)
        {
            GameObject specialObject = Instantiate(pair.Prefab, pair.Position, Quaternion.identity);
            specialObject.transform.SetParent(specialObjectsParent.transform, true);
            currentSpecialObjects.Add(specialObject);
            bool isActive = currentRoom.GetActiveObject(currentSpecialObjects.Count - 1);
            specialObject.GetComponent<TerrainObject>().Initialize(isActive, DeactivateSpecialObject);
        }

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

        if (!currentRoom.IsRoomCleared || currentRoom.IsChestOpened)
        {
            if (existingChest != null)
            {
                Destroy(existingChest);
                existingChest = null;
            }
        }
        else if (currentRoom.IsRoomCleared && !currentRoom.IsChestOpened)
        {
            if (existingChest == null)
            {
                SpawnChest();
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
        enemyQuota = enemySpawners.Count;
        enemySpawners.ForEach(spawner => spawner.GetComponent<EnemySpawner>().SpawnEnemy(enemies[Random.Range(0, enemies.Length)]));
    }

    public void IncrementEnemyQuota()
    {
        currentEnemyQuota++;

        if (currentEnemyQuota < enemyQuota) return;
        currentRoom.IsRoomCleared = true;
        if (Random.value < chestSpawnChance) SpawnChest();
        else SetChestOpened();
        onRoomCleared?.Invoke();
    }

    public void SetChestOpened()
    {
        currentRoom.IsChestOpened = true;
        existingChest = null;
    }

    public void DeactivateSpecialObject(GameObject specialObject)
    {
        int index = currentSpecialObjects.IndexOf(specialObject);
        currentRoom.DeactivateObject(index);
    }
}

public enum Direction { North, East, South, West, None }

public class DungeonRoom
{
    bool isRoomCleared, isChestOpened;

    List<Direction> exits;
    List<bool> activeObjects;
    Vector2 position;
    DungeonRoomLayout layout;

    public bool IsRoomCleared { get => isRoomCleared; set => isRoomCleared = value; }
    public bool IsChestOpened { get => isChestOpened; set => isChestOpened = value; }

    public Vector2 Position { get => position; }
    public List<Direction> Exits { get => exits; }
    public DungeonRoomLayout DungeonRoomLayout { get => layout; set => layout = value; }

    public DungeonRoom(Vector2 position, List<Direction> exits, DungeonRoomLayout layout)
    {
        this.position = position;
        this.exits = exits;
        this.layout = layout;

        bool isSafe = layout.EnemySpawnPositions.Count == 0;
        isRoomCleared = isSafe;
        isChestOpened = isSafe;

        activeObjects = layout.SpecialObjectPositions.Select(x => true).ToList();
    }

    public void AddExit(Direction direction)
    {
        if (exits.Contains(direction)) return;
        exits.Add(direction);
    }

    public void DeactivateObject(int index)
    {
        activeObjects[index] = false;
    }

    public bool GetActiveObject(int index)
    {
        return activeObjects[index];
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