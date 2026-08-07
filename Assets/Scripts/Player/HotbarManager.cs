using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] UnityEvent<int> onActivateSkill;

    int hotbarIndex;
    float[] skillCooldowns, hotbarCooldowns;

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
        hotbarCooldowns = new float[4];
        skillCooldowns = new float[4];
        inventory = player.GetComponent<Inventory>();
        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    void Update()
    {
        for (int i = 0; i < skillCooldowns.Length; i++)
        {
            if (skillCooldowns[i] > 0) skillCooldowns[i] -= Time.deltaTime;
            if (hotbarCooldowns[i] > 0) hotbarCooldowns[i] -= Time.deltaTime;
        }
    }

    void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < 4; i++)
        {
            Equipment equipInst = inventory.GetEquipmentAtIndex(i + 4);
            skillBar[i] = equipInst?.EquipmentSO as SkillTomeSO;
        }
        for (int i = 0; i < 4; i++)
        {
            ItemStack itemStack = inventory.GetHotbarItemAtIndex(i);
            hotbar[i] = itemStack?.Item.ItemSO as ConsumablesSO;
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
        if (hotbarCooldowns[hotbarIndex] > 0) return;
        currentConsumable.ActivateEffect(this);
        inventory.RemoveHotbarItem(currentConsumable);
        hotbarCooldowns[hotbarIndex] = currentConsumable.Cooldown;
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
