using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    DungeonGenerator managerScript;
    GameObject prefabEnemy;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndAnimation()
    {
        GameObject enemy = Instantiate(prefabEnemy, transform.position, Quaternion.identity);
        Health script = enemy.GetComponent<Health>();
        script.OnDeath.AddListener(managerScript.IncrementEnemyQuota);
    }

    public void SpawnEnemy(GameObject prefabEnemy)
    {
        this.prefabEnemy = prefabEnemy;
        animator.CrossFade("SpawnEnemy", 0, 0);
    }

    public void Initialize()
    {
        managerScript = GetComponentInParent<DungeonGenerator>();
        animator = GetComponent<Animator>();
    }
}
