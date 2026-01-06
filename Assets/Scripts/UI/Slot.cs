using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Sprite emptySprite;
    [SerializeField] protected Image backgroundImage, image;
    [SerializeField] Color highlightedColor, unhighlightedColor;

    protected int index;
    protected bool isEquiped, isEquipment;

    public void Highlight()
    {
        //backgroundImage.color = highlightedColor;
    }

    public void Unhighlight()
    {
        //backgroundImage.color = unhighlightedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image.sprite == emptySprite) return;
        EventManager.InvokeOnFocusItem(index, isEquiped, transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image.sprite == emptySprite) return;
        EventManager.InvokeOnUnfocusItem();
    }
}
