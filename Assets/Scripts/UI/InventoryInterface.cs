using System.Collections;
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
        EventManager.AddOnDropItemListener((int inte, SlotType slotType) => {
            DropItem(inte, slotType);
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
            ItemStack item = inventory.GetHotbarItem(i);
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();

            if (item == null) slot.ResetItem();
            else slot.UpdateItem(item.Item.Sprite, item.Amount);
        }
        for (int i = 4; i < equipmentSlots.childCount; i++)
        {
            Equipment item = inventory.GetEquipment(i - 4);
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();

            if (item == null) slot.ResetItem();
            else slot.UpdateItem(item.EquipmentData.Sprite, 1);
        }
        for (int i = 0; i < itemSlots.childCount; i++)
        {
            ItemStack stack = inventory.GetItem(i);
            ItemSlot slot = itemSlots.GetChild(i).GetComponent<ItemSlot>();

            if (stack == null || stack.Amount <= 0 || !stack.Item) slot.ResetItem();
            else slot.UpdateItem(stack.Item.Sprite, stack.Amount);
        }

        UpdateStatScreen();
    }

    void PickupItem(int index, SlotType slotType)
    {
        lastHeldItemIndex = index;
        lastHeldItemSlotType = slotType;
    }

    void RemoveOldItem(int index, SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Equipment: inventory.RemoveEquipmentAtIndex(index); break;
            case SlotType.Item: inventory.RemoveItemAtIndex(index); break;
        }
    }

    void DropItem(int index, SlotType slotType)
    {
        if (index == lastHeldItemIndex && slotType == lastHeldItemSlotType) return;
        ItemStack oldItem = null;
        ItemStack newItem = null;
        switch (lastHeldItemSlotType)
        {
            case SlotType.Equipment: oldItem = inventory.GetEquipAsItemStack(inventory.GetEquipment(lastHeldItemIndex)); break;
            case SlotType.Item: oldItem = inventory.GetItem(lastHeldItemIndex); break;
            case SlotType.Consumable: oldItem = inventory.GetHotbarItem(lastHeldItemIndex); break;
        }

        if (oldItem == null || !oldItem.Item) return;
        ItemStack temp = null;
        switch (slotType)
        {
            case SlotType.Equipment: temp = inventory.GetEquipAsItemStack(inventory.GetEquipment(index)); break;
            case SlotType.Item: temp = inventory.GetItem(index); break;
            case SlotType.Consumable: temp = inventory.GetHotbarItem(index); break;
        }

        if (temp != null && (slotType == SlotType.Equipment || lastHeldItemSlotType == SlotType.Equipment) && oldItem.Item as EquipmentSO == null != (temp.Item as EquipmentSO == null)) return;

        switch (slotType)
        {
            case SlotType.Equipment:
                EquipmentSO oldEquipmentSO = oldItem.Item as EquipmentSO;
                if (!oldEquipmentSO) return;

                Equipment toPlace = new (oldEquipmentSO);
                Equipment displaced = inventory.AddEquipmentAtIndex(toPlace, index);

                if (displaced == null) { RemoveOldItem(lastHeldItemIndex, lastHeldItemSlotType); return; }
                if (displaced == toPlace) return;
                newItem = new ItemStack(displaced.EquipmentData, 1);
                break;
            case SlotType.Item:
                ItemStack output = inventory.AddItemAtIndex(oldItem.Item, index, oldItem.Amount);

                if (output == null) { RemoveOldItem(lastHeldItemIndex, lastHeldItemSlotType); return; }
                if (output == oldItem) return;
                newItem = output;
                break;
            case SlotType.Consumable:
                ItemStack consumable = inventory.AddItemAtIndex(oldItem.Item, index, oldItem.Amount);

                if (consumable == null) { RemoveOldItem(lastHeldItemIndex, lastHeldItemSlotType); return; }
                if (consumable == oldItem) return;
                newItem = consumable;
                break;
        }

        switch (lastHeldItemSlotType)
        {
            case SlotType.Equipment:
                EquipmentSO newEquipmentSO = newItem.Item as EquipmentSO;
                if (newEquipmentSO) inventory.AddEquipmentAtIndex(new (newEquipmentSO), lastHeldItemIndex);
                break;
            case SlotType.Item:
                inventory.AddItemAtIndex(newItem.Item, lastHeldItemIndex, newItem.Amount);
                break;
            case SlotType.Consumable:
                ConsumablesSO consumablesSO = newItem.Item as ConsumablesSO;
                if (consumablesSO) inventory.AddHotbarItemAtIndex(newItem, lastHeldItemIndex);
                break;
                
        }
    }
}
