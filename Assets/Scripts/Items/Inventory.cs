using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    static Inventory inventory;

    public static Inventory Instance { get => inventory; }

    [SerializeField] int max;
    [SerializeField] Canvas itemCanvas;
    [SerializeField] Transform itemParent;
    [SerializeField] EquipmentEffectsManager equipmentEffectsManager;
    [SerializeField] Equipment[] startingEquipment;
    [SerializeField] InventorySlot[] startingItems;

    int itemCount;

    InventorySlot[] items;
    Equipment[] equiped;

    public IList<InventorySlot> Items { get => items.AsReadOnlyList(); }
    public IList<Equipment> Equiped { get => equiped.AsReadOnlyList(); }

    private void Awake()
    {
        inventory = this;
    }

    private void Start()
    {
        Initialize();
        UpdateInventory();

        EventManager.InvokeOnInventoryUpdated();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemObject itemObj = col.gameObject.GetComponent<ItemObject>();
        
        if (!itemObj) return;
        if (!AddItem(itemObj.Item)) return;
        Destroy(col.gameObject);
    }

    void Initialize()
    {
        equiped = new Equipment[12];
        foreach (Equipment equipment in startingEquipment)
        {
            int start = GetEquipmentIndex(equipment);
            for (int i = start; i < start + 4; i++)
            {
                if (equiped[i]) continue;
                equiped[i] = equipment;
                break;
            }
        }

        items = new InventorySlot[max];
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }
    }

    int GetEquipmentIndex(Item item)
    {
        if (item as Weapon) return 0;
        if (item as Armor) return 4;
        if (item as Accessory) return 8;
        return -1;
    }

    public bool EquipItem(Item item)
    {
        // Creating specific equipment types for later
        Weapon weapon = item as Weapon;
        Armor armor = item as Armor;
        Accessory accessory = item as Accessory;
        
        // Figuring out start index for correct index searching later
        int startIndex = GetEquipmentIndex(item);

        // Exiting function is item isn't equipment
        if (startIndex == -1) return false;

        // Finding index
        int index = 0;
        if (armor)
        {
            index = startIndex + (int)armor.ArmorPiece;
        }
        else
        {
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                if (equiped[startIndex] != null) continue;
                index = i;
            }
        }

        // Set bonuses for accessories
        if (accessory)
        {
            int setCount = 1;
            SetBonus curPieceSet = accessory.SetBonus;
            foreach (Equipment equip in equiped)
            {
                if (equip is not Accessory) continue;
                SetBonus setBonus = (equip as Accessory).SetBonus;
                if (setBonus == curPieceSet) setCount++;
            }

            if (setCount >= 2) curPieceSet.TwoPieceBonus();
            if (setCount >= 4) curPieceSet.FourPieceBonus();
        }

        // Moving equipment from items into equipment array
        RemoveItem(item);
        equiped[index] = item as Equipment;

        // Telling everybody else to update
        UpdateInventory();
        EventManager.InvokeOnInventoryUpdated();
        
        // Done
        return true;
    }

    public void UpdateInventory()
    {
        equipmentEffectsManager.RemoveAllEffects();

        equiped.ToList().ForEach(equip =>
        {
            if (!equip) return;
            Accessory accessory = equip as Accessory;

            if (!accessory) return;
            PassiveEffect passiveEffect = accessory.PassiveEffectSO.Initialize(gameObject, equipmentEffectsManager);
            equipmentEffectsManager.AddPassiveEffect(passiveEffect);
        });
    }

    public void UpdateItems(InventorySlot[] items)
    {
        this.items = items;
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (itemCount == max) return false;
        for (int i = 0; i < max; i++)
        {
            InventorySlot slot = items[i];
            if (slot == null)
            {
                items[i] = new (item, amount);
            }
            else
            {
                if (slot.Item != item) continue;
                slot.AddItems(amount);
            }
            return true;
        }
        return false;
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        for (int i = 0; i < max; i++)
        {
            InventorySlot slot = items[i];

            if (slot.Item != item) continue;
            if (slot.Amount < amount) return false;
            else if (slot.Amount == amount)
            {
                items[i] = null;
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