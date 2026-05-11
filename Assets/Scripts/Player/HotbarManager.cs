using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HotbarManager : MonoBehaviour
{
    ConsumablesSO currentConsumable;
    ConsumablesSO[] hotbar;
    SkillTomeSO[] skillBar;
    HotbarSlot[] hotbarUI, skillBarUI;

    int hotbarIndex;

    void Awake()
    {
        hotbarIndex = 0;
        hotbar = new ConsumablesSO[4];
        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < 4; i++)
        {
            ConsumablesSO consumable = inventory.GetEquipment(i) as ConsumablesSO;
            hotbar[i] = consumable;
            hotbarUI[i].Initialize(consumable.Sprite, consumable.Cooldown);
        }
        UpdateHotbarIndex(hotbarIndex);

        for (int i = 4; i < 8; i++)
        {
            SkillTomeSO skillTome = inventory.GetEquipment(i) as SkillTomeSO;
            skillBar[i - 4] = skillTome;
            skillBarUI[i - 4].Initialize(skillTome.Sprite, skillTome.Cooldown);
        }
    }

    public void UpdateHotbarIndex(int index)
    {
        int tempIndex = Mathf.Max(index - 1, 0);
        if (!hotbar[tempIndex]) return;
        hotbarIndex = tempIndex;
        currentConsumable = hotbar[hotbarIndex];
    }
}
