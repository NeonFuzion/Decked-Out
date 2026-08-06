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
    protected bool isEmpty;
    protected SlotType slotType;

    protected virtual void SetAmount(int amount)
    {
        amountText.SetText(amount <= 1 ? "" : amount.ToString());
    }

    protected virtual void FocusOnItem()
    {
        Inventory inventory = Inventory.Instance;
        ItemStack output = null;
        switch (slotType)
        {
            case SlotType.Equipment: output = inventory.GetEquipAsItemStack(inventory.GetEquipment(index)); break;
            case SlotType.Item: output = inventory.GetItem(index); break;
            case SlotType.Consumable: output = inventory.GetHotbarItem(index); break;
        }

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

    public virtual void Initialize(int index, SlotType slotType)
    {
        this.index = index;
        this.slotType = slotType;
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

public enum SlotType { None, Item, Equipment, Consumable }
