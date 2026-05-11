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
    [SerializeField] EquipmentSO[] startingEquipment;
    [SerializeField] ItemStack[] startingItems;

    int itemCount;

    ItemStack[] items;
    EquipmentSO[] equiped;

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
        equiped = new EquipmentSO[12];
        foreach (EquipmentSO equipment in startingEquipment)
        {
            int start = EquipmentSO.GetEquipmentIndex(equipment);
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
            ArmorSO armor = equip as ArmorSO;

            if (!armor) return;
            PassiveEffect passiveEffect = armor.PassiveEffectSO.Initialize(gameObject, equipmentEffectsManager);
            equipmentEffectsManager.AddPassiveEffect(passiveEffect);
        });
    }

    public ItemStack GetItem(int index)
    {
        return items[index];
    }

    public EquipmentSO GetEquipment(int index)
    {
        EquipmentSO equipment = equiped[index];

        if (equipment) return equipment;
        return null;
    }

    public int GetItemCount() => items.Length;
    public int GetEquipmentCount() => equiped.Length;

    public ItemStack FindItem(ItemSO item)
    {
        return items.ToList().Find(stack => stack != null && stack.Item == item);
    }

    public void UpdateItems(ItemStack[] items)
    {
        this.items = items;
    }

    public EquipmentSO AddEquipmentAtIndex(EquipmentSO equipment, int index)
    {
        if (!equipment) return null;

        // Creating specific equipment types for later
        ConsumablesSO mainHand = equipment as ConsumablesSO;
        ArmorSO armor = equipment as ArmorSO;
        SkillTomeSO skillTome = equipment as SkillTomeSO;

        int armorIndex = EquipmentSO.GetEquipmentIndex(armor);
        int skillTomeIndex = EquipmentSO.GetEquipmentIndex(skillTome);
        int mainHandIndex = EquipmentSO.GetEquipmentIndex(mainHand);

        // Filtering for incorrect equipment index
        if (armor && index != armorIndex + (int)armor.ArmorPiece) return equipment;
        if (skillTome && (index >= skillTomeIndex + 4 || index < skillTomeIndex)) return equipment;
        if (mainHand && (index >= mainHandIndex + 4 || index < mainHandIndex)) return equipment;
        if (!armor && !skillTome && !mainHand) return equipment;

        // Moving equipment from items into equipment array
        EquipmentSO oldItem = equiped[index];
        equiped[index] = equipment;
        return oldItem;
    }

    public EquipmentSO AddEquipment(EquipmentSO equipment)
    {
        // Setting useful variables
        int index = -1;
        int startIndex = EquipmentSO.GetEquipmentIndex(equipment);
        ArmorSO armor = equipment as ArmorSO;

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
        if (index == -1) return equipment;
        EquipmentSO oldEquipment = equiped[index];
        equiped[index] = equipment;
        return oldEquipment;
    }

    public ItemStack AddItemAtIndex(ItemSO item, int index, int amount = 1)
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
            if (item as EquipmentSO) return new (item);
            slot.AddItems(amount);
            return null;
        }
        else
        {
            items[index] = new (item, amount);
            return slot;
        }
    }

    public bool AddItem(ItemSO item, int amount = 1)
    {
        if (!item) return false;
        EquipmentSO equipment = item as EquipmentSO;

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

    public ItemStack RemoveItem(ItemSO item, int amount = -1)
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

    public EquipmentSO RemoveEquipment(EquipmentSO target)
    {
        for (int i = 0; i < equiped.Length; i++)
        {
            EquipmentSO equipment = equiped[i];

            if (equipment != target) continue;
            EquipmentSO result = RemoveEquipmentAtIndex(i);
            if (result) return equipment;
        }
        return null;
    }

    public EquipmentSO RemoveEquipmentAtIndex(int index)
    {
        EquipmentSO result = equiped[index];
        equiped[index] = null;
        return result;
    }

    public ItemStack GetEquipAsStack(EquipmentSO equipment)
    {
        if (!equipment) return null;
        return new (equipment);
    }
}

[System.Serializable]
public class ItemStack
{
    [SerializeField] ItemSO item;
    [SerializeField] int amount;

    public ItemSO Item { get => item; }
    public int Amount { get => amount; }

    public ItemStack(ItemSO item, int amount = 1)
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