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
        EventManager.OnInventoryUpdated.AddListener(UpdateInventory);
        EventManager.OnPickupItem.AddListener(PickupItem);
        EventManager.OnDropItem.AddListener((int index, SlotType slotType) => {
            DropItem(index, slotType);
            EventManager.OnInventoryUpdated.Invoke();
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
        ItemStack oldItem = lastHeldItemSlotType switch
        {
            SlotType.Equipment => ItemStack.ToStack(inventory.GetEquipmentAtIndex(lastHeldItemIndex)),
            SlotType.Item => inventory.GetItemAtIndex(lastHeldItemIndex),
            SlotType.Consumable => inventory.GetHotbarItemAtIndex(lastHeldItemIndex),
            _ => null
        };
        ItemStack newItem = slotType switch
        {
            SlotType.Equipment => ItemStack.ToStack(inventory.GetEquipmentAtIndex(index)),
            SlotType.Item => inventory.GetItemAtIndex(index),
            SlotType.Consumable => inventory.GetHotbarItemAtIndex(index),
            _ => null
        };

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

        bool areSlotsEqual = slotType == lastHeldItemSlotType;
        if (trueNewItemSlotType != SlotType.None && trueNewItemSlotType != trueOldItemSlotType && !areSlotsEqual) return;
        bool isSuccessful = slotType switch
        {
            SlotType.Equipment => inventory.AddEquipmentAtIndex(oldItem.Item as Equipment, index, out _, areSlotsEqual),
            SlotType.Consumable => inventory.AddHotbarItemAtIndex(oldItem, index, out _),
            SlotType.Item => inventory.AddItemAtIndex(oldItem, index, out _),
            _ => false
        };

        if (!isSuccessful) return;
        if (newItem != null && newItem.Item != null)
        {
            switch (lastHeldItemSlotType)
            {
                case SlotType.Equipment: inventory.AddEquipmentAtIndex(newItem.Item as Equipment, lastHeldItemIndex, out _, areSlotsEqual); break;
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
