using UnityEngine;

public class Equipment : Item
{
    public static int GetEquipmentIndex(Item item)
    {
        if (item as MainHand) return 0;
        if (item as Armor) return 4;
        if (item as SkillTomeSO) return 8;
        return -1;
    }
}
