using TMPro;
using UnityEngine;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] Transform equipmentSlots, itemSlots;
    [SerializeField] ItemFocus itemFocus;
    [SerializeField] TextMeshProUGUI statsDisplay;
    [SerializeField] Color32[] inventoryColors;

    Player player;
    Inventory inventory;

    bool isLastHeldItemEquiped;
    int lastHeldItemIndex;

    void Awake()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        EventManager.AddOnPickupItemListener(PickupItem);
        EventManager.AddOnDropItemListener((int inte, bool boole) => {
            DropItem(inte, boole); 
            EventManager.InvokeOnInventoryUpdated();
            });
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
    }

    void RemoveOldItem(int index, bool isEquiped)
    {
        if (isEquiped) inventory.RemoveEquipmentAtIndex(index);
        else inventory.RemoveItemAtIndex(index);
    }

    void DropItem(int index, bool isEquiped)
    {
        if (index == lastHeldItemIndex && isEquiped == isLastHeldItemEquiped) return;

        ItemStack oldItem = isLastHeldItemEquiped ? new (inventory.GetEquipment(lastHeldItemIndex)) : inventory.GetItem(lastHeldItemIndex);
        ItemStack newItem = null;

        if (!oldItem.Item) return;
        // Checking to see if both oldItem and newItem are both equipment
        ItemStack temp = isEquiped ? new (inventory.GetEquipment(index)) : inventory.GetItem(index);

        if (temp != null && temp.Item && (isEquiped || isLastHeldItemEquiped) && (oldItem.Item as Equipment == null) != (temp.Item as Equipment == null)) return;

        // Moving old item into new slot
        if (isEquiped)
        {
            Equipment oldEquipment = oldItem.Item as Equipment;

            if (!oldEquipment) return;
            Equipment newEquipment = inventory.AddEquipmentAtIndex(oldEquipment, index);

            if (!newEquipment)
            {
                RemoveOldItem(lastHeldItemIndex, isLastHeldItemEquiped);
                return;
            }
            if (newEquipment == oldEquipment) return;
            newItem = new (newEquipment);
        }
        else
        {
            ItemStack output = inventory.AddItemAtIndex(oldItem.Item, index, oldItem.Amount);

            if (output == null)
            {
                RemoveOldItem(lastHeldItemIndex, isLastHeldItemEquiped);
                return;
            }
            if (output == oldItem) return;
            newItem = output;
        }

        // Moving new item into old slot
        if (isLastHeldItemEquiped)
        {
            inventory.AddEquipmentAtIndex(newItem.Item as Equipment, lastHeldItemIndex);
        }
        else
        {
            inventory.AddItemAtIndex(newItem.Item, lastHeldItemIndex, newItem.Amount);
        }
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

        itemFocus.DisplayItemStats(item, itemAmount);
    }
}
