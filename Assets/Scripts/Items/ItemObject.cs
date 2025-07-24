using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] Item item;

    Transform player;
    LayerMask playerLayer;

    bool chase;

    public Item Item { get => item; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!chase) return;
        if (!player)
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, 3, playerLayer);
            if (!col) return;

            player = col.transform;
        }
        transform.Translate((player.position - transform.position).normalized * Time.deltaTime * 10);

        if (!Physics2D.OverlapCircle(transform.position, 0.1f, playerLayer)) return;
        chase = false;
    }

    public void Instantiate(Item item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = item.Sprite;
        EventManager.AddOnRoomChangedListener(() => Destroy(gameObject));
        playerLayer = LayerMask.GetMask("Player");
        chase = true;
    }
}