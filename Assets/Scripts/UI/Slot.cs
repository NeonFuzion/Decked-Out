using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected Image backgroundImage, image;
    [SerializeField] protected GameObject emptyImage;

    protected int index;
    protected bool isEquiped, isEmpty;

    protected virtual void SetAmount(int amount)
    {
        amountText.SetText(amount <= 1 ? "" : amount.ToString());
    }

    protected virtual void FocusOnItem()
    {
        Inventory inventory = Inventory.Instance;
        ItemStack output = isEquiped ? inventory.GetEquipAsStack(inventory.GetEquipment(index)) : inventory.GetItem(index);
        
        if (output == null) return;
        EventManager.InvokeOnFocusItem(output);
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
        FocusOnItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEmpty) return;
        EventManager.InvokeOnUnfocusItem();
    }
}
