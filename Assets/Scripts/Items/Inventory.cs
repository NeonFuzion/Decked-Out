using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas itemCanvas;
    [SerializeField] Transform itemParent, equipmentEffects;
    [SerializeField] Equipment[] equiped;
    [SerializeField] InventorySlots items;

    public Equipment[] Equiped { get => equiped; }
    public InventorySlots Items { get => items; }

    private void Start()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        UpdateInventory(equiped, items);
        EventManager.InvokeOnEquipmentUpdated(equiped);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemObject itemObj = col.gameObject.GetComponent<ItemObject>();
        if (!itemObj) return;
        AddItem(itemObj.Item);
        Destroy(col.gameObject);
    }

    public void AddItem(Item item)
    {
        items.AddItem(item);
    }

    public void EquipItem(Item item)
    {
        Equipment equipment = item as Equipment;
        if (!equipment) return;

        EquipmentEffect equipEffect = equipment.EquipmentEffect;
        if (equipEffect) equipEffect.Instantiate(gameObject);

        int index = 0;
        Accessory relic = equipment as Accessory;
        if (relic)
        {
            index = 1 + (int)relic.RelicSlot;

            int setCount = 1;
            SetBonus curPieceSet = relic.SetBonus;
            foreach (Equipment equip in equiped)
            {
                if (equip is not Accessory) continue;
                SetBonus setBonus = (equip as Accessory).SetBonus;
                if (setBonus == curPieceSet) setCount++;
            }

            if (setCount >= 2) curPieceSet.TwoPieceBonus();
            if (setCount >= 4) curPieceSet.FourPieceBonus();
        }

        if (!equiped[index]) items.RemoveItem(item);
        else items.AddItem(equiped[index]);
        equiped[index] = equipment;

        EventManager.InvokeOnEquipmentUpdated(equiped);
    }

    public void UpdateInventory(Equipment[] equiped, InventorySlots items)
    {
        this.equiped = equiped;
        this.items = items;

        foreach (Transform child in equipmentEffects) Destroy(child.gameObject);
        foreach (Equipment equip in equiped)
        {
            if (!equip) continue;
            EquipmentEffect equipmentEffect = equip.EquipmentEffect;

            if (!equipmentEffect) continue;
            equipmentEffect.Instantiate(gameObject);
        }
    }
}
