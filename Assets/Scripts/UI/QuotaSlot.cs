using UnityEngine;
using UnityEngine.Events;

public class QuotaSlot : Slot
{
    [SerializeField] Color insufficientColor;

    int quota;

    ItemStack itemStack;

    protected override void SetAmount(int amount)
    {
        amountText.SetText($"{amount} / {quota}");
        amountText.color = amount >= quota ? Color.black : insufficientColor;
    }

    public void Initialize(ItemStack itemStack, int amount)
    {
        quota = itemStack.Amount;
        this.itemStack = itemStack;
        
        SetAmount(amount);
        UpdateItem(itemStack.Item.Sprite, amount);
    }

    protected override void FocusOnItem()
    {
        EventManager.InvokeOnFocusItem(itemStack);
    }
}
