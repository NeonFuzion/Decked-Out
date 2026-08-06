using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemSlot : Slot, IDropHandler
{
    [SerializeField] UnityEvent<int, SlotType> onSetData;

    public override void Initialize(int index, SlotType slotType)
    {
        base.Initialize(index, slotType);
        onSetData?.Invoke(index, slotType);
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.InvokeOnDropItem(index, slotType);
    }
}
