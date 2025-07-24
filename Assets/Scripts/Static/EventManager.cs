using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static UnityEvent<Equipment[], InventorySlots> onInventoryUpdated = new UnityEvent<Equipment[], InventorySlots>();
    static UnityEvent<Equipment[]> onEquipmentUpdated = new UnityEvent<Equipment[]>();
    static UnityEvent<DialogueData[]> onDialogueStarted = new UnityEvent<DialogueData[]>();
    static UnityEvent<int, bool, Transform> onFocusItem = new UnityEvent<int, bool, Transform>();
    static UnityEvent<int, bool> onEquipItem = new UnityEvent<int, bool>();
    static UnityEvent onRoomCleared = new UnityEvent();
    static UnityEvent onKill = new UnityEvent();
    static UnityEvent onRoomChanged = new UnityEvent();
    static UnityEvent onCombatStarted = new UnityEvent();
    static UnityEvent onCombatEnded = new UnityEvent();

    public static void AddOnInventoryUpdatedListener(UnityAction<Equipment[], InventorySlots> listener)
    {
        onInventoryUpdated?.AddListener(listener);
    }

    public static void InvokeOnInventoryUpdated(Equipment[] equiped, InventorySlots items)
    {
        onInventoryUpdated?.Invoke(equiped, items);
    }

    public static void AddOnDialogueStartedListener(UnityAction<DialogueData[]> listener)
    {
        onDialogueStarted?.AddListener(listener);
    }

    public static void InvokeOnDialogueStarted(DialogueData[] data)
    {
        onDialogueStarted?.Invoke(data);
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

    public static void AddOnRoomChangedListener(UnityAction action)
    {
        onRoomChanged?.AddListener(action);
    }

    public static void InvokeOnRoomChanged()
    {
        onRoomChanged?.Invoke();
    }

    public static void AddOnCombatStartedListener(UnityAction action)
    {
        onCombatStarted?.AddListener(action);
    }

    public static void InvokeOnCombatStarted()
    {
        onCombatStarted?.Invoke();
    }

    public static void AddOnCombatEndedListener(UnityAction action)
    {
        onCombatEnded?.AddListener(action);
    }

    public static void InvokeOnCombatEnded()
    {
        onCombatEnded?.Invoke();
    }
}
