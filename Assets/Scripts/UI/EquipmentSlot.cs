using UnityEngine;

public class EquipmentSlot : Slot
{
    public void Initialize(int index)
    {
        this.index = index;
        isEquiped = true;
        image.SetNativeSize();
    }

    public void UpdateSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void ResetSprite()
    {
        image.sprite = emptySprite;
    }
}
