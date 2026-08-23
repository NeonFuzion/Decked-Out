using UnityEngine;
using UnityEngine.Events;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] Direction direction;
    [SerializeField] Transform playerSpawn;
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] UnityEvent onEnter, onGateUnlocked;

    BoxCollider2D boxCollider;
    Animator animator;
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
        EventManager.OnRoomChanged.Invoke();
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
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetBarrier()
    {
        if (gameObject.activeInHierarchy) animator.CrossFade("GateIdle", 0, 0);
        spriteRenderer.color = new (1, 1, 1, 0);
        boxCollider.isTrigger = true;
    }

    public void UnlockBarrier()
    {
        if (!gameObject.activeInHierarchy) return;
        animator.CrossFade("GateUnlock", 0, 0);
        onGateUnlocked?.Invoke();
    }

    public void LockBarrier()
    {
        if (!gameObject.activeInHierarchy) return;
        animator.CrossFade("GateLock", 0, 0);
    }
}
