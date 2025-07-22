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

[System.Serializable]
public class InventorySlots
{
    [SerializeField] List<InventorySlot> items;

    public List<InventorySlot> Items { get => items; }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item is not Equipment)
        {
            foreach (InventorySlot slot in items)
            {
                if (slot.Item != item) continue;
                slot.AddItems(amount);
                return false;
            }
        }
        items.Add(new InventorySlot(item, 1));
        return true;
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in items)
        {
            if (slot.Item != item) continue;
            if (slot.Amount < amount) return false;
            else if (slot.Amount == amount)
            {
                items.Remove(slot);
                return true;
            }
            else if (slot.Amount > amount)
            {
                slot.RemoveItems(amount);
                return true;
            }
        }
        return false;
    }

    public InventorySlot FindItem(Item target)
    {
        foreach (InventorySlot slot in items)
        {
            if (slot.Item != target) continue;
            return slot;
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot
{
    [SerializeField] Item item;
    [SerializeField] int amount;

    public Item Item { get => item; }
    public int Amount { get => amount; }

    public InventorySlot(Item item, int amount = 1)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddItems(int amount)
    {
        this.amount += amount;
    }

    public void RemoveItems(int amount)
    {
        this.amount -= amount;
    }
}