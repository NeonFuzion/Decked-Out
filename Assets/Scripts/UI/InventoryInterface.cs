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

    bool isLastHeldItemEquiped;
    int lastHeldItemIndex;

    void Awake()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        EventManager.AddOnPickupItemListener(PickupItem);
        EventManager.AddOnDropItemListener((int inte, bool boole) => {
            DropItem(inte, boole);
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

        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            EquipmentInstance item = inventory.GetEquipment(i);
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

        ItemStack oldItem = isLastHeldItemEquiped
            ? inventory.GetEquipAsItemStack(inventory.GetEquipment(lastHeldItemIndex))
            : inventory.GetItem(lastHeldItemIndex);
        ItemStack newItem = null;

        if (oldItem == null || !oldItem.Item) return;

        ItemStack temp = isEquiped
            ? inventory.GetEquipAsItemStack(inventory.GetEquipment(index))
            : inventory.GetItem(index);

        if (temp != null && (isEquiped || isLastHeldItemEquiped) && oldItem.Item as EquipmentSO == null != (temp.Item as EquipmentSO == null)) return;

        if (isEquiped)
        {
            EquipmentSO oldEquipmentSO = oldItem.Item as EquipmentSO;
            if (!oldEquipmentSO) return;

            EquipmentInstance toPlace = new EquipmentInstance(oldEquipmentSO);
            EquipmentInstance displaced = inventory.AddEquipmentAtIndex(toPlace, index);

            if (displaced == null) { RemoveOldItem(lastHeldItemIndex, isLastHeldItemEquiped); return; }
            if (displaced == toPlace) return;
            newItem = new ItemStack(displaced.EquipmentData, 1);
        }
        else
        {
            ItemStack output = inventory.AddItemAtIndex(oldItem.Item, index, oldItem.Amount);

            if (output == null) { RemoveOldItem(lastHeldItemIndex, isLastHeldItemEquiped); return; }
            if (output == oldItem) return;
            newItem = output;
        }

        if (isLastHeldItemEquiped)
        {
            EquipmentSO newEquipmentSO = newItem.Item as EquipmentSO;
            if (newEquipmentSO) inventory.AddEquipmentAtIndex(new EquipmentInstance(newEquipmentSO), lastHeldItemIndex);
        }
        else
        {
            inventory.AddItemAtIndex(newItem.Item, lastHeldItemIndex, newItem.Amount);
        }
    }
}
