using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected Image backgroundImage, image;
    [SerializeField] protected GameObject emptyImage;
    [SerializeField] Color highlightedColor, unhighlightedColor;

    protected int index;
    protected bool isEquiped, isEmpty;

    protected virtual void SetAmount(int amount)
    {
        amountText.SetText(amount <= 1 ? "" : amount.ToString());
    }

    public virtual void UpdateItem(Sprite sprite, int amount)
    {
        isEmpty = false;

        image.sprite = sprite;
        image.SetNativeSize();
        image.gameObject.SetActive(true);
        emptyImage.SetActive(false);

        if (!amountText) return;
        amountText.gameObject.SetActive(true);
        SetAmount(amount);
    }

    public virtual void ResetItem()
    {
        isEmpty = true;
        image.gameObject.SetActive(false);
        emptyImage.SetActive(true);
        
        if (!amountText) return;
        amountText.gameObject.SetActive(false);
    }

    public virtual void Initialize(int index, bool isEquiped)
    {
        this.index = index;
        this.isEquiped = isEquiped;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEmpty) return;
        EventManager.InvokeOnFocusItem(index, isEquiped, transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEmpty) return;
        EventManager.InvokeOnUnfocusItem();
    }
}
