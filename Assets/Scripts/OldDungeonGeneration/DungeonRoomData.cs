using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonRoomData : MonoBehaviour
{
    [SerializeField] GameObject prefabChest;
    [SerializeField] UnityEvent onRoomCleared;

    BoxCollider2D boxCollider;
    List<GameObject> enemies;
    List<Item> chestLoot;

    int enemiesRemaining;
    bool willSpawnChest;

    public UnityEvent OnRoomCleared { get => onRoomCleared; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (GameObject enemy in enemies)
        {
            if (!enemy) return;
            enemy.SetActive(true);
        }
    }

    void IncrementEnemyDeaths()
    {
        enemiesRemaining--;

        if (enemiesRemaining > 0) return;
        onRoomCleared?.Invoke();
        EventManager.InvokeOnRoomCleared();

        if (!willSpawnChest) return;
        GameObject chest = Instantiate(prefabChest, transform.position, Quaternion.identity);
        chest.GetComponent<LootDrops>().CommonDrops = chestLoot;
    }

    public void Instantiate(int size, List<GameObject> enemies, int chestChance, List<Item> chestLoot)
    {
        this.enemies = enemies;
        this.chestLoot = chestLoot;
        enemiesRemaining = enemies.Count;
        willSpawnChest = Random.Range(0, 100) <= chestChance;

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = Vector2.one * 2 * size;

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Health>().OnDeath.AddListener(IncrementEnemyDeaths);
            enemy.SetActive(false);
        }
    }
}
