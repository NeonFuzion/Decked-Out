using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] ItemStack[] startingItems;

    int itemCount;

    ItemStack[] items;
    Equipment[] equiped;

    private void Awake()
    {
        inventory = this;
        Initialize();
    }

    private void Start()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
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
            int start = Equipment.GetEquipmentIndex(equipment);
            for (int i = start; i < start + 4; i++)
            {
                if (equiped[i]) continue;
                equiped[i] = equipment;
                break;
            }
        }

        items = new ItemStack[max];
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }
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

            // Set bonuses for accessories
            if (!accessory.SetBonus) return;
            int setCount = 1;
            SetBonus curPieceSet = accessory.SetBonus;
            foreach (Equipment otherEquip in equiped)
            {
                if (otherEquip is not Accessory) continue;
                SetBonus setBonus = (otherEquip as Accessory).SetBonus;
                if (setBonus == curPieceSet) setCount++;
            }

            if (setCount >= 2) curPieceSet.TwoPieceBonus();
            if (setCount >= 4) curPieceSet.FourPieceBonus();
        });
    }

    public ItemStack GetItem(int index)
    {
        ItemStack stack = items[index];

        if (stack == null) return null;
        if (!stack.Item) return null;
        return stack;
    }

    public Equipment GetEquipment(int index)
    {
        return equiped[index];
    }

    public int GetItemCount() => items.Length;
    public int GetEquipmentCount() => equiped.Length;

    public ItemStack FindItem(Item item)
    {
        return items.ToList().Find(stack => stack.Item == item);
    }

    public void UpdateItems(ItemStack[] items)
    {
        this.items = items;
    }

    public Equipment AddEquipmentAtIndex(Equipment equipment, int index)
    {
        if (!equipment) return null;

        // Creating specific equipment types for later
        MainHand mainHand = equipment as MainHand;
        Armor armor = equipment as Armor;
        Accessory accessory = equipment as Accessory;

        int armorIndex = Equipment.GetEquipmentIndex(armor);
        int accessoryIndex = Equipment.GetEquipmentIndex(accessory);
        int mainHandIndex = Equipment.GetEquipmentIndex(mainHand);

        // Filtering for incorrect equipment index
        if (armor && index != armorIndex + (int)armor.ArmorPiece) return equipment;
        if (accessory && (index >= accessoryIndex + 4 || index < accessoryIndex)) return equipment;
        if (mainHand && (index >= mainHandIndex + 4 || index < mainHandIndex)) return equipment;
        if (!armor && !accessory && !mainHand) return equipment;

        // Moving equipment from items into equipment array
        Equipment oldItem = equiped[index];
        equiped[index] = equipment;
        return oldItem;
    }

    public bool AddEquipment(Equipment equipment)
    {
        // Setting useful variables
        int index = -1;
        int startIndex = Equipment.GetEquipmentIndex(equipment);
        Armor armor = equipment as Armor;

        // Finding correct index
        if (armor)
        {
            index = startIndex + (int)armor.ArmorPiece;
        }
        else
        {
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                if (equiped[i] != null) continue;
                index = i;
                break;
            }
        }

        // Adding equipment if index is found
        if (index == -1) return false;
        equiped[index] = equipment;
        return true;
    }

    public ItemStack AddItemAtIndex(Item item, int index, int amount = 1)
    {
        if (!item) return null;
        ItemStack slot = items[index];

        if (slot == null)
        {
            items[index] = new (item, amount);
            return null;
        }
        else if (slot.Item == item)
        {
            if (item as Equipment) return new (item);
            slot.AddItems(amount);
            return null;
        }
        else
        {
            items[index] = new (item, amount);
            return slot;
        }
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (!item) return false;
        Equipment equipment = item as Equipment;

        if (equipment && itemCount == max) return false;
        int index = -1;
        bool isSlotNull = true;
        for (int i = 0; i < max; i++)
        {
            ItemStack slot = items[i];
            if (slot == null)
            {
                index = i;
                isSlotNull = true;
                break;
            }
            else if (slot.Item == item && !equipment)
            {
                index = i;
                isSlotNull = false;
                break;
            }
        }

        if (index == -1) return false;
        if (isSlotNull) items[index] = new (item, amount);
        else items[index].AddItems(amount);
        return true;
    }

    public ItemStack RemoveItem(Item item, int amount = -1)
    {
        for (int i = 0; i < max; i++)
        {
            ItemStack slot = items[i];
            if (slot.Item != item) continue;
            ItemStack result = RemoveItemAtIndex(i, amount);
            if (result != null) return result;
        }
        return null;
    }

    public ItemStack RemoveItemAtIndex(int index, int amount = -1)
    {
        ItemStack stack = items[index];

        if (stack == null) return null;
        if (stack.Amount < amount) return null;
        else if (stack.Amount == amount || amount == -1)
        {
            items[index] = null;
            return stack;
        }
        else if (stack.Amount > amount)
        {
            stack.RemoveItems(amount);
            return stack;
        }
        return null;
    }

    public Equipment RemoveEquipment(Equipment target)
    {
        for (int i = 0; i < equiped.Length; i++)
        {
            Equipment equipment = equiped[i];

            if (equipment != target) continue;
            Equipment result = RemoveEquipmentAtIndex(i);
            if (result) return equipment;
        }
        return null;
    }

    public Equipment RemoveEquipmentAtIndex(int index)
    {
        Equipment result = equiped[index];
        equiped[index] = null;
        return result;
    }
}

[System.Serializable]
public class ItemStack
{
    [SerializeField] Item item;
    [SerializeField] int amount;

    public Item Item { get => item; }
    public int Amount { get => amount; }

    public ItemStack(Item item, int amount = 1)
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