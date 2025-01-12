using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonContentGenerator : MonoBehaviour
{
    [SerializeField] int roomSize;
    [SerializeField] GameObject prefabDungeonRoom, prefabFountain;
    [SerializeField] List<GameObject> enemies;
    [SerializeField] List<Item> chestLoot;
    [SerializeField] UnityEvent onEnemySpawn;

    float spawnTime, curSpawnTime;
    int enemyCount, enemyMax;
    bool canSpawn;

    Vector2 screenRes;

    public int EnemyCount { get => enemyCount; }
    public bool CanSpawn { get => canSpawn; set => canSpawn = value; }

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 5;
        curSpawnTime = 0;
        enemyCount = 0;
        enemyMax = 5;
        canSpawn = true;

        screenRes = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RoomSpawn(Vector2 roomCenter)
    {
        Vector2 spawnPos = roomCenter;

        int size = roomSize - 3;
        GameObject dungeonRoom = Instantiate(prefabDungeonRoom, roomCenter, Quaternion.identity);
        List<GameObject> roomSpawns = new List<GameObject>();
        for (int i = 0; i < Random.Range(3, 5); i++)
        {
            spawnPos += new Vector2(Random.Range(-size, size), Random.Range(-size, size));
            roomSpawns.Add(Instantiate(enemies[Random.Range(0, enemies.Count)], spawnPos, Quaternion.identity));
        }
        DungeonRoomData script = dungeonRoom.GetComponent<DungeonRoomData>();
        script.Instantiate(roomSize, roomSpawns, 30, chestLoot);
    }

    void CreateFountain(Vector2 roomCenter)
    {
        GameObject fountain = Instantiate(prefabFountain, roomCenter, Quaternion.identity);
        EventManager.AddOnRoomClearedListener(fountain.GetComponent<Fountain>().AddRoomCount);
    }

    public void GenerateRoomContents(List<Vector2Int> roomOrigins)
    {
        foreach (Vector2Int origin in roomOrigins)
        {
            if (origin == Vector2.zero) continue;
            if (Random.Range(0, 7) == 0) CreateFountain(origin);
            else RoomSpawn(origin);
        }
    }
}
