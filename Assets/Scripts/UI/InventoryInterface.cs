using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject, prefabItemSlot, prefabEquipSlot;
    [SerializeField] Transform equipmentSlots, itemSlots;
    [SerializeField] TextMeshProUGUI statsDisplay;

    InventorySlots items;
    Equipment[] equiped;
    Transform lastSelected;
    Player player;
    Color32[] inventoryColors;

    // Start is called before the first frame update
    void Start()
    {
        inventoryColors = new Color32[] {
            new Color32(255, 255, 200, 255),
            new Color32(255, 240, 170, 255),
            new Color32(255, 220, 120, 255),
            new Color32(255, 200, 80, 255)
        };

        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnEquipListener(EquipItem);
    }

    private void OnEnable()
    {
        if (items != null && equiped != null) return;
        Inventory script = playerGameObject.GetComponent<Inventory>();
        player = script.GetComponent<Player>();
        items = script.Items;
        equiped = script.Equiped;
    }

    public void FocusOnItem(int index, bool isEquiped, Transform itemSlot)
    {
        int itemAmount = 1;
        Item item;
        if (isEquiped)
        {
            item = equiped[index];
        }
        else
        {
            InventorySlot slot = items.Items[index];
            item = slot.Item;
            itemAmount = slot.Amount;
        }

        Transform description = transform.GetChild(2);

        if (lastSelected && lastSelected != itemSlot) lastSelected.GetComponentInChildren<Image>().color = inventoryColors[2];
        lastSelected = itemSlot;
        lastSelected.GetComponentInChildren<Image>().color = inventoryColors[3];

        string equipmentDetails = "\n";
        string descriptionText = $"{item.ItemName} {(itemAmount > 1 ? $"x{itemAmount}\n" : equipmentDetails)}";

        if (item as Weapon)
        {
            equipmentDetails += $"Weapon\nAttack - {(item as Weapon).Attack}\n\n";
        }
        else if (item as Accessory)
        {
            Accessory accessory = item as Accessory;
            equipmentDetails += $"{accessory.RelicSlot}\n{accessory.MainStat} - {accessory.StatBoost}%\n\n";
        }

        string effectDetails = "";
        if (item as Equipment)
        {
            Equipment equipment = item as Equipment;
            if (equipment.EquipmentEffect)
            {
                effectDetails += $"{equipment.EquipmentEffect.Description}\n\n";
            }
        }

        descriptionText += $"{equipmentDetails}{effectDetails}{item.Description}";
        description.GetComponentInChildren<TextMeshProUGUI>().SetText(descriptionText);

        Image img = description.GetChild(0).GetComponent<Image>();
        img.sprite = item.Sprite;
        img.color = Color.white;
        img.SetNativeSize();
    }

    public void UpdateInventory()
    {
        foreach (Transform child in itemSlots) Destroy(child.gameObject);
        foreach (Transform child in equipmentSlots) Destroy(child.gameObject);

        for (int i = 0; i < equiped.Length; i++)
        {
            if (!equiped[i]) continue;
            GameObject equipSlot = Instantiate(prefabEquipSlot, equipmentSlots);
            equipSlot.GetComponent<ItemSlot>().Initialize(equiped[i].Sprite, 1, i, true, true);
        }
        for (int i = 0; i < items.Items.Count; i++)
        {
            InventorySlot slot = items.Items[i];
            GameObject itemSlot = Instantiate(prefabItemSlot, itemSlots);
            itemSlot.GetComponent<ItemSlot>().Initialize(slot.Item.Sprite, slot.Amount, i, slot.Item as Equipment, false);
            itemSlot.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
        }

        EventManager.InvokeOnInventoryUpdated(equiped, items);
        EventManager.InvokeOnEquipmentUpdated(equiped);
        statsDisplay.SetText(player.GetStats());
    }

    public void EquipItem(int index, bool isEquiped)
    {
        if (isEquiped)
        {
            items.AddItem(equiped[index]);
            equiped[index] = null;
        }
        else
        {
            Equipment equipment = items.Items[index].Item as Equipment;
            int equipmentSlot = 1;
            if (equipment as Weapon) equipmentSlot--;
            else equipmentSlot += (int)(equipment as Accessory).RelicSlot;

            if (equiped[equipmentSlot])
            {
                items.Items[index] = new InventorySlot(equiped[equipmentSlot]);
                equiped[equipmentSlot] = equipment;
            }
            else
            {
                equiped[equipmentSlot] = equipment;
                items.RemoveItem(equipment);
            }
        }
        UpdateInventory();
    }
}
