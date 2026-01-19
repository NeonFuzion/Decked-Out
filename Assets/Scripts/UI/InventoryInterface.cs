using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] Transform equipmentSlots, itemSlots;
    [SerializeField] ItemFocus itemFocus;
    [SerializeField] TextMeshProUGUI statsDisplay;
    [SerializeField] Color32[] inventoryColors;

    Transform lastSelected;
    Player player;
    Inventory inventory;

    bool isHoldingItem, isLastHeldItemEquiped;
    int lastHeldItemIndex;

    void Awake()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        EventManager.AddOnPickupItemListener(PickupItem);
        EventManager.AddOnDropItemListener(DropItem);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        
    }

    void Initialize()
    {
        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();
            slot.Initialize(i, true);
        }
        for (int i = 0; i < itemSlots.childCount; i++) 
        {
            ItemSlot slot = itemSlots.GetChild(i).GetComponent<ItemSlot>();
            slot.Initialize(i, false);
        }
    }

    void UpdateInventory()
    {
        if (!inventory)
        {
            inventory = Inventory.Instance;
            player = inventory.GetComponent<Player>();
        }

        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            Equipment item = inventory.GetEquipment(i);
            ItemSlot slot = equipmentSlots.GetChild(i).GetComponent<ItemSlot>();

            if (item == null) slot.ResetItem();
            else slot.UpdateItem(item.Sprite, 1);
        }
        for (int i = 0; i < itemSlots.childCount; i++) 
        {
            ItemStack stack = inventory.GetItem(i);
            ItemSlot slot = itemSlots.GetChild(i).GetComponent<ItemSlot>();

            if (stack == null || stack.Amount <= 0 || !stack.Item) slot.ResetItem();
            else slot.UpdateItem(stack.Item.Sprite, stack.Amount);
        }

        statsDisplay.SetText(player.GetStats());
    }

    void PickupItem(int index, bool isEquiped)
    {
        lastHeldItemIndex = index;
        isLastHeldItemEquiped = isEquiped;
        isHoldingItem = true;
    }

    void DropItem(int index, bool isEquiped)
    {
        //Debug.Log($"{lastHeldItemIndex} + {isLastHeldItemEquiped} -> {index} + {isEquiped}");
        isHoldingItem = false;

        if (index == lastHeldItemIndex && isEquiped == isLastHeldItemEquiped) return;

        ItemStack oldItem = isLastHeldItemEquiped ? new (inventory.GetEquipment(lastHeldItemIndex)) : inventory.GetItem(lastHeldItemIndex);
        ItemStack newItem = isEquiped ? new (inventory.GetEquipment(index)) : inventory.GetItem(index);
        
        if (newItem != null && !newItem.Item) newItem = null;
        //Debug.Log($"{oldItem != null} -> {newItem != null}");

        // A bunch of filters to prevent equiping materials
        if (isEquiped && !(oldItem.Item as Equipment)) return;
        if (newItem != null && isLastHeldItemEquiped && !(newItem.Item as Equipment)) return;

        if (isLastHeldItemEquiped)
        {
            inventory.RemoveEquipmentAtIndex(lastHeldItemIndex);
        }
        else
        {
            inventory.RemoveItemAtIndex(lastHeldItemIndex);
        }
        
        if (isEquiped)
        {
            inventory.RemoveEquipmentAtIndex(index);
            inventory.AddEquipmentAtIndex(oldItem.Item as Equipment, index);
        }
        else
        {
            inventory.RemoveItemAtIndex(index);
            inventory.AddItemAtIndex(oldItem.Item, index, oldItem.Amount);
        }

        if (newItem == null)
        {
            EventManager.InvokeOnInventoryUpdated();
            return;
        }
        
        if (isLastHeldItemEquiped)
        {
            inventory.AddEquipmentAtIndex(newItem.Item as Equipment, lastHeldItemIndex);
        }
        else
        {
            inventory.AddItemAtIndex(newItem.Item, lastHeldItemIndex, newItem.Amount);
        }
        EventManager.InvokeOnInventoryUpdated();
    }

    void FocusOnItem(int index, bool isEquiped, Transform itemSlot)
    {
        int itemAmount = 1;
        Item item;
        if (isEquiped)
        {
            item = inventory.GetEquipment(index);
        }
        else
        {
            if (inventory.GetItem(index) == null) return;
            ItemStack slot = inventory.GetItem(index);
            item = slot.Item;
            itemAmount = slot.Amount;
        }

        if (lastSelected && lastSelected != itemSlot) lastSelected.GetComponent<Slot>().Unhighlight();
        lastSelected = itemSlot;
        lastSelected.GetComponent<Slot>().Highlight();

        itemFocus.DisplayItemStats(item, itemAmount);
    }
}
