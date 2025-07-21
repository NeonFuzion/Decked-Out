using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject, prefabItemSlot, prefabEquipSlot;
    [SerializeField] Transform equipmentSlots, itemSlots, description;
    [SerializeField] TextMeshProUGUI statsDisplay, focusName, focusType, focusAmount, focusStats, focusDescription, focusAbility;
    [SerializeField] Color32[] inventoryColors;

    InventorySlots items;
    Equipment[] equiped;
    Transform lastSelected;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnEquipListener(EquipItem);
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

        if (lastSelected && lastSelected != itemSlot) lastSelected.GetComponentInChildren<Image>().color = inventoryColors[2];
        lastSelected = itemSlot;
        lastSelected.GetComponentInChildren<Image>().color = inventoryColors[3];

        focusAmount.gameObject.SetActive(itemAmount > 1);
        if (itemAmount > 1)
        {
            focusAmount.SetText($"x{itemAmount}");
            focusType.SetText("Material");
        }

        focusName.SetText(item.ItemName);

        focusStats.gameObject.SetActive(false);
        focusAbility.gameObject.SetActive(false);

        if (item as Weapon)
        {
            focusType.SetText("Weapon");
            focusStats.SetText($"Attack - {(item as Weapon).Attack}");
        }
        else if (item as Accessory)
        {
            Accessory accessory = item as Accessory;
            focusType.SetText($"Accessory");
            focusStats.SetText($"{accessory.MainStat.Stat} - {accessory.MainStat.Amount}%");
        }

        if (item as Equipment)
        {
            focusStats.gameObject.SetActive(true);

            Equipment equipment = item as Equipment;
            EquipmentEffect ability = equipment.EquipmentEffect;
            focusAbility.gameObject.SetActive(ability);

            if (ability)
            {
                focusAbility.SetText($"{ability.Name}:\n{ability.Description}");
            }
        }

        focusDescription.SetText(item.Description);

        Image img = description.GetChild(1).GetChild(0).GetComponent<Image>();
        img.sprite = item.Sprite;
        img.color = Color.white;
        img.SetNativeSize();
    }

    public void UpdateInventory()
    {
        foreach (Transform child in itemSlots) Destroy(child.gameObject);

        SetInventory();

        for (int i = 0; i < equipmentSlots.childCount; i++)
        {
            Transform equipSlot = equipmentSlots.GetChild(i);
            EquipmentSlot script = equipSlot.GetComponent<EquipmentSlot>();

            if (!equiped[i]) script.ResetSprite();
            else script.UpdateSprite(equiped[i].Sprite);
        }
        for (int i = 0; i < items.Items.Count; i++)
        {
            InventorySlot slot = items.Items[i];
            GameObject itemSlot = Instantiate(prefabItemSlot, itemSlots);
            itemSlot.GetComponent<ItemSlot>().Initialize(slot.Item.Sprite, slot.Amount, i, slot.Item as Equipment);

            Transform itemImage = itemSlot.transform.GetChild(0);
            itemImage.GetComponent<Image>().SetNativeSize();
            itemImage.GetComponent<RectTransform>().sizeDelta *= 3;
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
            int equipmentSlot = -1;
            if (equipment as Weapon) equipmentSlot = 0;
            else
            {
                for (int i = 3; i < equiped.Length; i++)
                {
                    if (equiped[i] != null) continue;
                    equipmentSlot = i;
                    break;
                }
            }

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

    public void SetInventory()
    {
        if (items != null && equiped != null) return;
        Inventory script = playerGameObject.GetComponent<Inventory>();
        player = script.GetComponent<Player>();
        items = script.Items;
        equiped = script.Equiped;
    }
}
