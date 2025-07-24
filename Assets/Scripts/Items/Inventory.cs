using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas itemCanvas;
    [SerializeField] Transform itemParent, equipmentEffects;
    [SerializeField] EquipmentEffectsManager effectsManager;
    [SerializeField] Equipment[] startingEquipment;
    [SerializeField] InventorySlots items;

    Equipment[] equiped;

    public Equipment[] Equiped { get => equiped; }
    public InventorySlots Items { get => items; }

    private void Start()
    {
        EquipStartingEquipment();
        UpdateInventory(equiped, items);

        EventManager.AddOnInventoryUpdatedListener(UpdateInventory);
        EventManager.InvokeOnEquipmentUpdated(equiped);

        GetComponent<Player>().UpdateStats(equiped);
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemObject itemObj = col.gameObject.GetComponent<ItemObject>();
        if (!itemObj) return;
        AddItem(itemObj.Item);
        Destroy(col.gameObject);
    }

    void EquipStartingEquipment()
    {
        equiped = new Equipment[9];
        foreach (Equipment equipment in startingEquipment)
        {
            if (equipment as Weapon)
            {
                equiped[0] = equipment;
            }
            else if (equipment as Accessory)
            {
                for (int i = 3; i < 9; i++)
                {
                    if (equiped[i]) continue;
                    equiped[i] = equipment;
                    break;
                }
            }
            else if (equipment as SkillCharm)
            {
                SkillCharm charm = equipment as SkillCharm;
                equiped[charm.CharmType == CharmType.Skill ? 1 : 2] = equipment;
            }
        }
    }

    public void AddItem(Item item, int amount = 1)
    {
        items.AddItem(item, amount);
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
            index = 3;
            while (true)
            {
                if (equiped[index++] != null || index >= equiped.Length) break; 
            }

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

        effectsManager.RemoveAllEffects();

        foreach (Transform child in equipmentEffects) Destroy(child.gameObject);
        for (int i = 0; i < 9; i++)
        {
            Equipment equip = equiped[i];

            if (!equip) continue;
            EquipmentEffect equipmentEffect = equip.EquipmentEffect;

            if (!equipmentEffect) continue;
            equipmentEffect.Instantiate(gameObject);
        }
    }
}


[System.Serializable]
public class InventorySlots
{
    [SerializeField] List<InventorySlot> items;

    public List<InventorySlot> Items { get => items; }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item is not Equipment)
        {
            foreach (InventorySlot slot in items)
            {
                if (slot.Item != item) continue;
                slot.AddItems(amount);
                return false;
            }
        }
        items.Add(new InventorySlot(item, 1));
        return true;
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        foreach (InventorySlot slot in items)
        {
            if (slot.Item != item) continue;
            if (slot.Amount < amount) return false;
            else if (slot.Amount == amount)
            {
                items.Remove(slot);
                return true;
            }
            else if (slot.Amount > amount)
            {
                slot.RemoveItems(amount);
                return true;
            }
        }
        return false;
    }

    public InventorySlot FindItem(Item target)
    {
        foreach (InventorySlot slot in items)
        {
            if (slot.Item != target) continue;
            return slot;
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot
{
    [SerializeField] Item item;
    [SerializeField] int amount;

    public Item Item { get => item; }
    public int Amount { get => amount; }

    public InventorySlot(Item item, int amount = 1)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddItems(int amount)
    {
        this.amount += amount;
    }

    public void RemoveItems(int amount)
    {
        this.amount -= amount;
    }
}