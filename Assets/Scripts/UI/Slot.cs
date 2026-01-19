using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected Image backgroundImage, image;
    [SerializeField] protected GameObject emptyImage;
    [SerializeField] Color highlightedColor, unhighlightedColor;

    protected int index;
    protected bool isEquiped, isEmpty;

    public void Highlight()
    {
        //backgroundImage.color = highlightedColor;
    }

    public void Unhighlight()
    {
        //backgroundImage.color = unhighlightedColor;
    }

    public virtual void UpdateItem(Sprite sprite, int amount)
    {
        isEmpty = false;

        image.sprite = sprite;
        image.SetNativeSize();
        image.gameObject.SetActive(true);
        emptyImage.SetActive(false);

        if (amount <= 1) return;
        amountText.SetText(amount.ToString());
    }

    public virtual void ResetItem()
    {
        isEmpty = true;
        image.gameObject.SetActive(false);
        emptyImage.SetActive(true);
    }

    public virtual void Initialize(int index, bool isEquiped)
    {
        this.index = index;
        this.isEquiped = isEquiped;
        //Debug.Log($"{gameObject.name}: {index} - {isEquiped}");
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
