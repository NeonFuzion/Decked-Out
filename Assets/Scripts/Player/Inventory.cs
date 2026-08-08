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
    [SerializeField] ItemStack[] startingItems, startingHotbar;

    int itemCount;

    ItemStack[] items, hotbar;
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

        ItemStack outItem;
        if (!AddItem(ItemStack.ToStack(itemObj.Item), out outItem)) return;
        Destroy(col.gameObject);
    }

    void Initialize()
    {
        equiped = new Equipment[8];
        Equipment outEquipment;
        foreach (EquipmentSO equipment in startingEquipment)
        {
            AddEquipment(new (equipment), out outEquipment);
        }

        hotbar = new ItemStack[4];
        for (int i = 0; i < startingHotbar.Length; i++)
        {
            hotbar[i] = ItemStack.ToStack(startingHotbar[i].Item.ItemSO, startingHotbar[i].Amount);
        }

        items = new ItemStack[max];
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = ItemStack.ToStack(startingItems[i].Item.ItemSO, startingItems[i].Amount);
        }
    }

    public void UpdateInventory()
    {
        equipmentEffectsManager.RemoveAllEffects();

        equiped.ToList().ForEach(equip =>
        {
            if (equip == null) return;
            ArmorSO armor = equip.EquipmentSO as ArmorSO;

            if (!armor || !armor.PassiveEffectSO) return;
            PassiveEffect passiveEffect = armor.PassiveEffectSO.Initialize(gameObject, equipmentEffectsManager);
            equipmentEffectsManager.AddPassiveEffect(passiveEffect);
        });
    }

    public ItemStack GetItemAtIndex(int index)
    {
        return items[index];
    }

    public ItemStack FindItem(ItemSO item)
    {
        return items.ToList().Find(stack => stack != null && stack.Item.ItemSO == item);
    }

    public Equipment GetEquipmentAtIndex(int index)
    {
        return equiped[index];
    }

    public ItemStack GetHotbarItemAtIndex(int index)
    {
        return hotbar[index];
    }

    public int GetItemCount() => items.Length;
    public int GetEquipmentCount() => equiped.Length;

    public void UpdateItems(ItemStack[] items)
    {
        this.items = items;
    }

    public bool AddEquipmentAtIndex(Equipment equipment, int index, out Equipment oldEquipment, bool allowDuplicates = false)
    {
        oldEquipment = null;
        if (equipment == null || equipment.EquipmentSO == null) return false;

        EquipmentSO data = equipment.EquipmentSO;
        ArmorSO armor = data as ArmorSO;
        SkillTomeSO skillTome = data as SkillTomeSO;

        int armorIndex = EquipmentSO.GetEquipmentIndex(armor);
        int skillTomeIndex = EquipmentSO.GetEquipmentIndex(skillTome);

        if (armor && index != armorIndex + (int)armor.ArmorPiece) return false;
        if (skillTome && ((equiped.Count(equip => equip?.EquipmentSO == skillTome) > 0 && !allowDuplicates) || index >= skillTomeIndex + 4 || index < skillTomeIndex)) return false;

        oldEquipment = equiped[index];
        equiped[index] = equipment;
        return true;
    }

    public bool AddEquipment(Equipment equipment, out Equipment oldEquipment)
    {
        oldEquipment = null;
        if (equipment == null || equipment.EquipmentSO == null) return false;
        int index = -1;
        int startIndex = EquipmentSO.GetEquipmentIndex(equipment.EquipmentSO);
        ArmorSO armor = equipment.EquipmentSO as ArmorSO;
        
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

        if (index == -1) return false;
        oldEquipment = equiped[index];
        equiped[index] = equipment;
        return true;
    }

    public bool AddHotbarItemAtIndex(ItemStack itemStack, int index, out ItemStack oldItemStack)
    {
        oldItemStack = null;
        if (itemStack == null) return false;
        ConsumablesSO consumable = itemStack.Item.ItemSO as ConsumablesSO;
        ItemStack hotbarItem = hotbar[index];

        if (!consumable) return false;
        if (hotbarItem != null && hotbarItem.Item.ItemSO == itemStack.Item.ItemSO)
        {
            hotbarItem.AddItems(itemStack.Amount);
            return true;
        }
        else
        {
            oldItemStack = hotbarItem;
            hotbar[index] = itemStack;
            return true;
        }
    }

    public bool AddHotbarItem(ItemStack itemStack, out ItemStack oldItemStack)
    {
        oldItemStack = null;
        for (int i = 0; i < hotbar.Length; i++)
        {
            ItemStack hotbarItem = hotbar[i];
            if (hotbarItem.Item.ItemSO == itemStack.Item.ItemSO)
            {
                hotbar[i].AddItems(itemStack.Amount);
                return true;
            }
            if (!hotbarItem.Item.ItemSO)
            {
                oldItemStack = hotbarItem;
                hotbar[i] = itemStack;
                return true;
            }
        }
        return false;
    }

    public bool AddItemAtIndex(ItemStack itemStack, int index, out ItemStack oldItemStack)
    {
        oldItemStack = null;
        if (itemStack.Item == null) return false;
        ItemStack slot = items[index];

        if (slot == null || slot.Item.ItemSO != itemStack.Item.ItemSO || itemStack.Item.ItemSO as EquipmentSO)
        {
            oldItemStack = slot;
            items[index] = itemStack;
        }
        else
        {
            slot.AddItems(itemStack.Amount);
        }
        return true;
    }

    public bool AddItem(ItemStack itemStack, out ItemStack oldItemStack)
    {
        oldItemStack = null;
        if (!itemStack.Item.ItemSO) return false;
        EquipmentSO equipment = itemStack.Item.ItemSO as EquipmentSO;

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
            else if (slot.Item.ItemSO == itemStack.Item.ItemSO && !equipment)
            {
                index = i;
                isSlotNull = false;
                break;
            }
        }

        if (index == -1) return false;
        if (isSlotNull)
        {
            oldItemStack = items[index];
            items[index] = itemStack;
        }
        else
        {
            items[index].AddItems(itemStack.Amount);
        }
        return true;
    }

    public ItemStack RemoveItem(ItemSO item, int amount = -1)
    {
        int index = items.ToList().FindIndex(itemStack => itemStack.Item.ItemSO == item);

        if (index == -1) return null;
        return RemoveItemAtIndex(index, amount);
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
            return new (stack.Item, amount);
        }
        return null;
    }

    public Equipment RemoveEquipment(EquipmentSO target)
    {
        int index = equiped.ToList().FindIndex(equipment => equipment.EquipmentSO == target);

        if (index == -1) return null;
        return RemoveEquipmentAtIndex(index);
    }

    public Equipment RemoveEquipmentAtIndex(int index)
    {
        Equipment result = equiped[index];
        equiped[index] = null;
        return result;
    }

    public ItemStack RemoveHotbarItem(ConsumablesSO target, int amount = -1)
    {
        int index = hotbar.ToList().FindIndex(itemStack => itemStack.Item.ItemSO == target);
        
        if (index == -1) return null;
        return RemoveHotbarItemAtIndex(index, amount);
    }

    public ItemStack RemoveHotbarItemAtIndex(int index, int amount = -1)
    {
        ItemStack output = hotbar[index];
        if (output == null) return null;
        if (amount > output.Amount) return null;
        if (amount == output.Amount || amount == -1)
        {
            hotbar[index] = null;
            return output;
        }
        if (amount < output.Amount)
        {
            hotbar[index].RemoveItems(amount);
            return new (output.Item, amount);
        }
        return null;
    }
}

[System.Serializable]
public class Item
{
    [SerializeField] ItemSO itemSO;

    public ItemSO ItemSO => itemSO;

    public Item(ItemSO itemSO)
    {
        this.itemSO = itemSO;
    }
}

[System.Serializable]
public class ItemStack
{
    [SerializeField] int amount;
    [SerializeField] Item item;

    public int Amount { get => amount; }
    public Item Item => item;

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

    public static ItemStack ToStack(Item item, int amount = 1)
    {
        if (item == null) return null;
        return new (item, amount);
    }

    public static ItemStack ToStack(ItemSO itemSO, int amount = 1)
    {
        if (!itemSO) return null;
        Item item = null;
        switch (itemSO)
        {
            case ArmorSO armorSO: item = new Equipment(armorSO); break;
            case SkillTomeSO skillTomeSO: item = new SkillTome(skillTomeSO); break;
            default: item = new (itemSO); break;
        }
        return new (item, amount);
    }
}

[System.Serializable]
public class Equipment : Item
{
    EquipmentSO equipmentSO;

    public EquipmentSO EquipmentSO => equipmentSO;

    public Equipment(ItemSO itemSO) : base (itemSO)
    {
        equipmentSO = itemSO as EquipmentSO;
    }
}