using UnityEngine;
using UnityEngine.Events;

public class DungeonRoomObject : MonoBehaviour
{
    [SerializeField] GameObject[] exitTilemaps, roomTransitions;
    [SerializeField] GameObject treasureChest;
    [SerializeField] UnityEvent onRoomCleared, onCombatInitiated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateRoom(DungeonRoom dungeonRoom)
    {

    }

    public void AddEnemyQuota()
    {
        
    }
}
