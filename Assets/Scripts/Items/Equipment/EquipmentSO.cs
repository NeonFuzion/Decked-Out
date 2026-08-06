using UnityEngine;

public class EquipmentSO : ItemSO
{
    public static int GetEquipmentIndex(ItemSO item)
    {
        if (item as ArmorSO) return 0;
        if (item as SkillTomeSO) return 4;
        return -1;
    }
}
