using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : TerrainObject
{
    DungeonGenerator managerScript;
    GameObject prefabEnemy;
    Animator animator;
    EnemySpawnerData enemySpawnerData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndAnimation()
    {
        GameObject enemy = Instantiate(prefabEnemy, transform.position, Quaternion.identity);
        Health script = enemy.GetComponent<Health>();
        script.OnDeath.AddListener(managerScript.IncrementCurrentEnemyQuota);
    }

    public void SpawnEnemy()
    {
        animator.CrossFade("SpawnEnemy", 0, 0);
        enemySpawnerData.IsActive = false;
    }

    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        prefabEnemy = dungeonGenerator.EnemyPool[Random.Range(0, dungeonGenerator.EnemyPool.Length)];
        return new EnemySpawnerData(true, prefabEnemy);
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        managerScript = dungeonGenerator;
        animator = GetComponent<Animator>();

        enemySpawnerData = roomObjectData as EnemySpawnerData;
        prefabEnemy = enemySpawnerData.PrefabEnemy;

        if (!enemySpawnerData.IsActive) return;
        dungeonGenerator.IsRoomCleared = false;
        dungeonGenerator.IncrementEnemyQuota();
        EventManager.AddOnCombatStartedListener(SpawnEnemy);
    }
}

public class EnemySpawnerData : RoomObjectData
{
    public GameObject PrefabEnemy;

    public EnemySpawnerData(bool isActive, GameObject prefabEnemy) : base(isActive)
    {
        PrefabEnemy = prefabEnemy;
    }
}