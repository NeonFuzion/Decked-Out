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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Initialize();
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
        ResetBarrier();
        onEnter?.Invoke();
    }

    public void Initialize()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetBarrier()
    {
        spriteRenderer.enabled = true;
        boxCollider.isTrigger = false;
    }

    public void UnlockBarrier()
    {
        spriteRenderer.enabled = false;
        boxCollider.isTrigger = true;
    }
}
