using TMPro;
using UnityEngine;

public class ItemSlot : Slot
{
    [SerializeField] TextMeshProUGUI amountText;

    public void Initialize(Sprite sprite, int amount, int index, bool isEquipment)
    {
        this.index = index;
        this.isEquipment = isEquipment;

        isEquiped = false;
        image.sprite = sprite;

        if (isEquipment || amount == 1) return;
        amountText.text = amount.ToString();
    }

    public override void OnLeftClick()
    {
        EventManager.InvokeOnFocusItem(index, false, transform);
    }

    public override void OnRightClick()
    {
        if (!isEquipment) return;
        EventManager.InvokeOnEquip(index, false);
    }
}
