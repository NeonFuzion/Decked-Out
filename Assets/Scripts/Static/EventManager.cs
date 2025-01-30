using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static UnityEvent<Equipment[], InventorySlots> onInventoryUpdated = new UnityEvent<Equipment[], InventorySlots>();
    static UnityEvent<Equipment[]> onEquipmentUpdated = new UnityEvent<Equipment[]>();
    static UnityEvent<int, bool, Transform> onFocusItem = new UnityEvent<int, bool, Transform>();
    static UnityEvent<int, bool> onEquipItem = new UnityEvent<int, bool>();
    static UnityEvent onRoomCleared = new UnityEvent();
    static UnityEvent onKill = new UnityEvent();

    public static void AddOnInventoryUpdatedListener(UnityAction<Equipment[], InventorySlots> listener)
    {
        onInventoryUpdated?.AddListener(listener);
    }

    public static void InvokeOnInventoryUpdated(Equipment[] equiped, InventorySlots items)
    {
        onInventoryUpdated?.Invoke(equiped, items);
    }

    public static void AddOnEquipmentUpdatedListener(UnityAction<Equipment[]> listener)
    {
        onEquipmentUpdated?.AddListener(listener);
    }

    public static void InvokeOnEquipmentUpdated(Equipment[] equiped)
    {
        onEquipmentUpdated?.Invoke(equiped);
    }

    public static void AddOnFocusItemListener(UnityAction<int, bool, Transform> focusAction)
    {
        onFocusItem?.AddListener(focusAction);
    }

    public static void InvokeOnFocusItem(int index, bool isEquipment, Transform item)
    {
        onFocusItem?.Invoke(index, isEquipment, item);
    }

    public static void AddOnEquipListener(UnityAction<int, bool> equipAction)
    {
        onEquipItem?.AddListener(equipAction);
    }

    public static void InvokeOnEquip(int index, bool isEquiped)
    {
        onEquipItem?.Invoke(index, isEquiped);
    }

    public static void AddOnRoomClearedListener(UnityAction action)
    {
        onRoomCleared?.AddListener(action);
    }

    public static void InvokeOnRoomCleared()
    {
        onRoomCleared?.Invoke();
    }

    public static void AddOnKillListener(UnityAction action)
    {
        onKill?.AddListener(action);
    }

    public static void InvokeOnKill()
    {
        onKill?.Invoke();
    }
}
