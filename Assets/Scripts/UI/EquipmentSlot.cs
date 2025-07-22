using UnityEngine;

public class EquipmentSlot : Slot
{
    [SerializeField] Sprite emptySprite;

    public void Initialize(int index)
    {
        this.index = index;
    }

    public void UpdateSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void ResetSprite()
    {
        image.sprite = emptySprite;
    }

    public override void OnLeftClick()
    {
        if (image.sprite == emptySprite) return;
        EventManager.InvokeOnFocusItem(index, true, transform);
    }

    public override void OnRightClick()
    {
        if (index == 0) return;
        ResetSprite();
        EventManager.InvokeOnEquip(index, true);
    }
}
