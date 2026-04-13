using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static UnityEvent onRoomCleared = new ();
    static UnityEvent onKill = new ();
    static UnityEvent onRoomChanged = new ();
    static UnityEvent onCombatStarted = new ();
    static UnityEvent onCombatEnded = new ();
    static UnityEvent onInventoryUpdated = new ();
    static UnityEvent onUnfocusItem = new ();
    static UnityEvent onOpenCraftingMenu = new ();
    static UnityEvent<int, bool> onPickupItem = new ();
    static UnityEvent<int, bool> onDropItem = new ();
    static UnityEvent<ItemStack> onFocusItem = new ();
    static UnityEvent<GameObject> onMenuOpened = new ();
    static UnityEvent<DialogueData[]> onDialogueStarted = new ();
    static UnityEvent<Collider2D[], AttackData> onEnemyDataAcquired = new ();

    public static void AddOnRoomClearedListener(UnityAction action) => onRoomCleared?.AddListener(action);
    public static void InvokeOnRoomCleared() => onRoomCleared?.Invoke();

    public static void AddOnKillListener(UnityAction action) => onKill?.AddListener(action);
    public static void InvokeOnKill() => onKill?.Invoke();

    public static void AddOnRoomChangedListener(UnityAction action) => onRoomChanged?.AddListener(action);
    public static void InvokeOnRoomChanged() => onRoomChanged?.Invoke();

    public static void AddOnCombatStartedListener(UnityAction action) => onCombatStarted?.AddListener(action);
    public static void InvokeOnCombatStarted() => onCombatStarted?.Invoke();

    public static void AddOnCombatEndedListener(UnityAction action) => onCombatEnded?.AddListener(action);
    public static void InvokeOnCombatEnded() => onCombatEnded?.Invoke();

    public static void AddOnMenuOpenedListener(UnityAction<GameObject> action) => onMenuOpened?.AddListener(action);
    public static void InvokeOnMenuOpened(GameObject gameObject) => onMenuOpened?.Invoke(gameObject);

    public static void AddOnInventoryUpdatedListener(UnityAction unityAction) => onInventoryUpdated?.AddListener(unityAction);
    public static void InvokeOnInventoryUpdated() => onInventoryUpdated?.Invoke();

    public static void AddOnUnfocusItemListener(UnityAction unityAction) => onUnfocusItem?.AddListener(unityAction);
    public static void InvokeOnUnfocusItem() => onUnfocusItem?.Invoke();

    public static void AddOnOpenCraftingMenu(UnityAction unityAction) => onOpenCraftingMenu?.AddListener(unityAction);
    public static void InvokeOnOpenCraftingMenu() => onOpenCraftingMenu?.Invoke();

    public static void AddOnPickupItemListener(UnityAction<int, bool> unityAction) => onPickupItem?.AddListener(unityAction);
    public static void InvokeOnPickupItem(int index, bool isEquiped) => onPickupItem?.Invoke(index, isEquiped);

    public static void AddOnDropItemListener(UnityAction<int, bool> unityAction) => onDropItem?.AddListener(unityAction);
    public static void InvokeOnDropItem(int index, bool isEquiped) => onDropItem?.Invoke(index, isEquiped);
    
    public static void AddOnFocusItemListener(UnityAction<ItemStack> focusAction) => onFocusItem?.AddListener(focusAction);
    public static void InvokeOnFocusItem(ItemStack itemStack) => onFocusItem?.Invoke(itemStack);

    public static void AddOnDialogueStartedListener(UnityAction<DialogueData[]> listener) => onDialogueStarted?.AddListener(listener);
    public static void InvokeOnDialogueStarted(DialogueData[] data) => onDialogueStarted?.Invoke(data);

    public static void AddOnEnemyDataAcquiredListener(UnityAction<Collider2D[], AttackData> listener) => onEnemyDataAcquired?.AddListener(listener);
    public static void InvokeOnEnemyDataAcquired(Collider2D[] colliders, AttackData attackData) => onEnemyDataAcquired?.Invoke(colliders, attackData);
}
/*
    public static void AddOnListener(UnityAction listener) => on.AddListener(listener);
    public static void InvokeOn() => on.Invoke();
*/