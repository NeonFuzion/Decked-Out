using UnityEngine;

public class EquipmentSO : ItemSO
{
    public static int GetEquipmentIndex(ItemSO item)
    {
        if (item as ConsumablesSO) return 0;
        if (item as ArmorSO) return 4;
        if (item as SkillTomeSO) return 8;
        return -1;
    }
}
