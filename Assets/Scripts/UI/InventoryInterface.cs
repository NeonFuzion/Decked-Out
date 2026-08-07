using System.Collections;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] Transform equipmentSlots, itemSlots;
    [SerializeField] TextMeshProUGUI statsDisplay;
    [SerializeField] Color32[] inventoryColors;

    Player player;
    Inventory inventory;

    SlotType lastHeldItemSlotType;
    int lastHeldItemIndex;

    void Awake()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        EventManager.AddOnPickupItemListener(PickupItem);
        EventManager.AddOnDropItemListener((int index, SlotType slotType) => {
            DropItem(index, slotType);
            EventManager.InvokeOnInventoryUpdated();
            });
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {

    }

    void Initialize()
    {
        for (int i = 0; i < 4; i++)
        {
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();
            slot.Initialize(i, SlotType.Consumable);
        }
        for (int i = 4; i < equipmentSlots.childCount; i++)
        {
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();
            slot.Initialize(i - 4, SlotType.Equipment);
        }
        for (int i = 0; i < itemSlots.childCount; i++)
        {
            ItemSlot slot = itemSlots.GetChild(i).GetComponent<ItemSlot>();
            slot.Initialize(i, SlotType.Item);
        }

        UpdateStatScreen();
    }

    void UpdateStatScreen()
    {
        statsDisplay.SetText(player.GetStats());
    }

    void UpdateInventory()
    {
        if (!inventory)
        {
            inventory = Inventory.Instance;
            player = inventory.GetComponent<Player>();
        }

        for (int i = 0; i < 4; i++)
        {
            ItemStack item = inventory.GetHotbarItemAtIndex(i);
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();

            if (item == null || item.Item == null) slot.ResetItem();
            else slot.UpdateItem(item.Item.ItemSO.Sprite, item.Amount);
        }
        for (int i = 4; i < equipmentSlots.childCount; i++)
        {
            Equipment item = inventory.GetEquipmentAtIndex(i - 4);
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();

            if (item == null) slot.ResetItem();
            else slot.UpdateItem(item.EquipmentSO.Sprite, 1);
        }
        for (int i = 0; i < itemSlots.childCount; i++)
        {
            ItemStack stack = inventory.GetItemAtIndex(i);
            ItemSlot slot = itemSlots.GetChild(i).GetComponent<ItemSlot>();

            if (stack == null || stack.Amount <= 0 || !stack.Item.ItemSO) slot.ResetItem();
            else slot.UpdateItem(stack.Item.ItemSO.Sprite, stack.Amount);
        }

        UpdateStatScreen();
    }

    void PickupItem(int index, SlotType slotType)
    {
        lastHeldItemIndex = index;
        lastHeldItemSlotType = slotType;
    }

    void DropItem(int index, SlotType slotType)
    {
        if (index == lastHeldItemIndex && slotType == lastHeldItemSlotType) return;
        ItemStack oldItem = null, newItem = null;
        switch (lastHeldItemSlotType)
        {
            case SlotType.Equipment: oldItem = ItemStack.ToStack(inventory.GetEquipmentAtIndex(lastHeldItemIndex)); break;
            case SlotType.Item: oldItem = inventory.GetItemAtIndex(lastHeldItemIndex); break;
            case SlotType.Consumable: oldItem = inventory.GetHotbarItemAtIndex(lastHeldItemIndex); break;
        }
        switch (slotType)
        {
            case SlotType.Equipment: newItem = ItemStack.ToStack(inventory.GetEquipmentAtIndex(index)); break;
            case SlotType.Item: newItem = inventory.GetItemAtIndex(index); break;
            case SlotType.Consumable: newItem = inventory.GetHotbarItemAtIndex(index); break;
        }

        SlotType trueOldItemSlotType = SlotType.None, trueNewItemSlotType = SlotType.None;
        switch (oldItem.Item.ItemSO)
        {
            case ConsumablesSO: trueOldItemSlotType = SlotType.Consumable; break;
            case SkillTomeSO: trueOldItemSlotType = SlotType.SkillTome; break;
            case ArmorSO: trueOldItemSlotType = SlotType.Armor; break;
            case ItemSO: trueOldItemSlotType = SlotType.Item; break;
        }
        switch (newItem?.Item.ItemSO)
        {
            case ConsumablesSO: trueNewItemSlotType = SlotType.Consumable; break;
            case SkillTomeSO: trueNewItemSlotType = SlotType.SkillTome; break;
            case ArmorSO: trueNewItemSlotType = SlotType.Armor; break;
            case ItemSO: trueNewItemSlotType = SlotType.Item; break;
        }

        if (trueNewItemSlotType != SlotType.None && trueNewItemSlotType != trueOldItemSlotType && slotType != lastHeldItemSlotType) return;
        bool isSuccessful = false;
        switch (slotType)
        {
            case SlotType.Equipment: isSuccessful = inventory.AddEquipmentAtIndex(oldItem.Item as Equipment, index, out _, true); break;
            case SlotType.Consumable: isSuccessful = inventory.AddHotbarItemAtIndex(oldItem, index, out _); break;
            case SlotType.Item: isSuccessful = inventory.AddItemAtIndex(oldItem, index, out _); break;
        }

        if (!isSuccessful) return;
        if (newItem != null && newItem.Item != null)
        {
            switch (lastHeldItemSlotType)
            {
                case SlotType.Equipment: inventory.AddEquipmentAtIndex(newItem.Item as Equipment, lastHeldItemIndex, out _, true); break;
                case SlotType.Consumable: inventory.AddHotbarItemAtIndex(newItem, lastHeldItemIndex, out _); break;
                case SlotType.Item: inventory.AddItemAtIndex(newItem, lastHeldItemIndex, out _); break;
            }
        }
        else
        {
            switch (lastHeldItemSlotType)
            {
                case SlotType.Equipment: inventory.RemoveEquipmentAtIndex(lastHeldItemIndex); break;
                case SlotType.Consumable: inventory.RemoveHotbarItemAtIndex(lastHeldItemIndex); break;
                case SlotType.Item: inventory.RemoveItemAtIndex(lastHeldItemIndex); break;
            }
        }
    }
}
