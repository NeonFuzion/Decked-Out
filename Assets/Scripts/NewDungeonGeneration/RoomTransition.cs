using UnityEngine;
using UnityEngine.Events;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] Direction direction;
    [SerializeField] Transform playerSpawn;
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] UnityEvent onEnter;

    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<Player>()) return;
        if (!boxCollider.isTrigger) return;
        dungeonGenerator.LoadRoom(direction);
    }

    public void SpawnPlayer(GameObject player)
    {
        player.transform.position = playerSpawn.position;
        if (spriteRenderer) spriteRenderer.enabled = true;
        if (boxCollider) boxCollider.isTrigger = false;
        onEnter?.Invoke();
    }

    public void UnlockBarrier()
    {
        if (!gameObject.activeInHierarchy) return;
        if (spriteRenderer) spriteRenderer.enabled = false;
        if (boxCollider) boxCollider.isTrigger = true;
    }
}
