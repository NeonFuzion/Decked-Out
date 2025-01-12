using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    DungeonGenerator managerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        managerScript = GetComponentInParent<DungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(GameObject prefabEnemy)
    {
        GameObject enemy = Instantiate(prefabEnemy, transform.position, Quaternion.identity);
        Health script = enemy.GetComponent<Health>();
        script.OnDeath.AddListener(managerScript.IncrementEnemyQuota);
    }
}
