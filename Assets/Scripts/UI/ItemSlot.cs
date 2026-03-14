using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemSlot : Slot, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] UnityEvent<int, bool> onSetData;

    public override void Initialize(int index, bool isEquiped)
    {
        base.Initialize(index, isEquiped);
        onSetData?.Invoke(index, isEquiped);
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.InvokeOnDropItem(index, isEquiped);
    }
}
