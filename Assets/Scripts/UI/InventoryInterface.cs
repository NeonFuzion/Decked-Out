using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    void Awake()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            equipmentSlots.GetChild(i).GetComponent<EquipmentSlot>().Initialize(i);
        }
    }

    public void FocusOnItem(int index, bool isEquiped, Transform itemSlot)
    {
        int itemAmount = 1;
        Item item;
        if (isEquiped)
        {
            item = inventory.Equiped[index];
        }
        else
        {
            InventorySlot slot = inventory.Items[index];
            item = slot.Item;
            itemAmount = slot.Amount;
        }

        if (lastSelected && lastSelected != itemSlot) lastSelected.GetComponent<Slot>().Unhighlight();
        lastSelected = itemSlot;
        lastSelected.GetComponent<Slot>().Highlight();

        itemFocus.gameObject.SetActive(true);
        itemFocus.DisplayItemStats(item, itemAmount);
    }

    public void UpdateInventory()
    {
        if (!inventory)
        {
            inventory = Inventory.Instance;
            player = inventory.GetComponent<Player>();
        }

        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            Transform equipSlot = equipmentSlots.GetChild(i);
            EquipmentSlot script = equipSlot.GetComponent<EquipmentSlot>();
            
            if (!inventory.Equiped[i]) script.ResetSprite();
            else script.UpdateSprite(inventory.Equiped[i].Sprite);

            script.Initialize(i);
        }
        for (int i = 0; i < inventory.Items.Count; i++) 
        {
            InventorySlot slot = inventory.Items[i];

            if (slot == null) continue;
            itemSlots.GetChild(i).GetComponent<ItemSlot>().Initialize(slot.Item.Sprite, slot.Amount, i, slot.Item as Equipment);
        }

        statsDisplay.SetText(player.GetStats());
    }

    public void EquipItem(int index, bool isEquiped)
    {
        if (isEquiped)
        {
            inventory.AddItem(inventory.Equiped[index]);
            inventory.Equiped[index] = null;
            equipmentSlots.GetChild(index).GetComponent<Slot>().Unhighlight();
        }
        else
        {
            Equipment equipment = inventory.Items[index].Item as Equipment;
            int equipmentSlot = -1;
            if (equipment as Weapon) equipmentSlot = 0;
            else if (equipment as Accessory && !inventory.Equiped.ToList().Find(x => x && x.Equals(equipment)))
            {
                for (int i = 3; i < inventory.Equiped.Count; i++)
                {
                    if (inventory.Equiped[i] != null) continue;
                    equipmentSlot = i;
                    break;
                }
            }

            if (equipmentSlot != -1)
            {
                if (inventory.Equiped[equipmentSlot])
                {
                    inventory.Items[index] = new InventorySlot(inventory.Equiped[equipmentSlot]);
                    inventory.Equiped[equipmentSlot] = equipment;
                }
                else
                {
                    inventory.Equiped[equipmentSlot] = equipment;
                    inventory.RemoveItem(equipment);
                }
            }
        }
        UpdateInventory();
    }
}
