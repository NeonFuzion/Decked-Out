using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent OnRoomCleared = new (), OnKill = new (), OnRoomChanged = new (), OnCombatStarted = new (), OnCombatEnded = new (), OnInventoryUpdated = new (), OnUnfocusItem = new (), OnOpenCraftingMenu = new ();
    public static UnityEvent<int, SlotType> OnPickupItem = new (), OnDropItem = new ();
    public static UnityEvent<ItemStack> OnFocusItem = new ();
    public static UnityEvent<GameObject> OnMenuOpened = new ();
    public static UnityEvent<DialogueData[]> OnDialogueStarted = new ();
    public static UnityEvent<Collider2D[], AttackData> OnEnemyDataAcquired = new ();
}