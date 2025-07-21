using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Image image;

    protected int index;
    protected bool isEquiped, isEquipment;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick();
        if (eventData.button == PointerEventData.InputButton.Right) OnRightClick();
    }

    public abstract void OnLeftClick();

    public abstract void OnRightClick();
}
