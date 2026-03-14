using UnityEngine;
using UnityEngine.Events;

public class QuotaSlot : Slot
{
    [SerializeField] Color insufficientColor;

    int quota;

    protected override void SetAmount(int amount)
    {
        amountText.SetText($"{amount} / {quota}");
        amountText.color = amount >= quota ? Color.black : insufficientColor;
    }

    public void Initialize(int index, bool isEquiped, int quota)
    {
        this.quota = quota;

        base.Initialize(index, isEquiped);
    }
}
