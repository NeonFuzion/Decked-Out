using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] UnityEvent<int> onActivateSkill;

    int hotbarIndex;
    float[] skillCooldowns, consumableCooldowns;

    ConsumablesSO currentConsumable;
    Inventory inventory;
    ConsumablesSO[] hotbar;
    SkillTomeSO[] skillBar;

    public Player Player => player;

    void Awake()
    {
        hotbarIndex = 0;
        hotbar = new ConsumablesSO[4];
        skillBar = new SkillTomeSO[4];
        skillCooldowns = new float[4];
        inventory = player.GetComponent<Inventory>();
        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    void Update()
    {
        for (int i = 0; i < skillCooldowns.Length; i++)
            if (skillCooldowns[i] > 0) skillCooldowns[i] -= Time.deltaTime;
    }

    void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < 4; i++)
        {
            Equipment equipInst = inventory.GetEquipment(4 + i);
            skillBar[i] = equipInst?.EquipmentData as SkillTomeSO;
        }
        for (int i = 0; i < 4; i++)
        {
            Equipment equipInst = inventory.GetEquipment(i);
            hotbar[i] = equipInst?.EquipmentData as ConsumablesSO;
        }
        UpdateHotbarIndex(hotbarIndex);
    }

    public void UpdateHotbarIndex(int index)
    {
        int tempIndex = Mathf.Clamp(index, 0, 3);
        if (!hotbar[tempIndex]) return;
        hotbarIndex = tempIndex;
        currentConsumable = hotbar[hotbarIndex];
    }

    public void ActivateSkill(int index)
    {
        if (index < 0 || index >= skillBar.Length) return;

        SkillTomeSO skillTomeSO = skillBar[index];
        if (skillTomeSO == null) return;
        if (skillCooldowns[index] > 0) return;
        if (!player.ConsumeMana(skillTomeSO.ResourceCost)) return;
        skillCooldowns[index] = skillTomeSO.Cooldown;
        skillTomeSO.ActivateEffects(player, 0);
        onActivateSkill?.Invoke(index);
    }

    public void UseConsumable()
    {
        currentConsumable.ActivateEffect(this);
        inventory.RemoveEquipment(currentConsumable);
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        RunCoroutine(coroutine);
    }
}
