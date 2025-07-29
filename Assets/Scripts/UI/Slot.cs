using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Image backgroundImage, image;
    [SerializeField] Color highlightedColor, unhighlightedColor;

    protected int index;
    protected bool isEquiped, isEquipment;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick();
        if (eventData.button == PointerEventData.InputButton.Right) OnRightClick();
    }

    public void Highlight()
    {
        //backgroundImage.color = highlightedColor;
    }

    public void Unhighlight()
    {
        //backgroundImage.color = unhighlightedColor;
    }

    public abstract void OnLeftClick();

    public abstract void OnRightClick();
}
