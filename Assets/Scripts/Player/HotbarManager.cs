using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Shooter shooter;
    [SerializeField] Transform skillParent;
    [SerializeField] UnityEvent<int> onActivateSkill, onUseConsumable;

    int hotbarIndex;
    float[] skillCooldowns, hotbarCooldowns;

    ConsumablesSO currentConsumable;
    Inventory inventory;
    ConsumablesSO[] hotbar;
    SkillTomeSO[] skillBar;

    ParticleSystem[] particleSystems;

    public Shooter Shooter => shooter;
    public Transform SkillParent => skillParent;

    void Awake()
    {
        hotbar = new ConsumablesSO[4];
        skillBar = new SkillTomeSO[4];
        particleSystems = new ParticleSystem[4];
        hotbarCooldowns = new float[4];
        skillCooldowns = new float[4];
        inventory = GetComponent<Inventory>();
        EventManager.OnInventoryUpdated.AddListener(UpdateHotbar);
        UpdateHotbarIndex(0);
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
            SkillTomeSO skillTome = equipInst?.EquipmentSO as SkillTomeSO;
            if (!skillTome) skillBar[i] = null;
            else if (skillBar[i] != skillTome)
            {
                skillBar[i] = skillTome;

                if (!skillTome.PrefabParticleSystem) continue;
                Destroy(particleSystems[i]?.gameObject);
                GameObject particleSystemHolder = Instantiate(skillTome.PrefabParticleSystem, skillParent);
                particleSystems[i] = particleSystemHolder.GetComponent<ParticleSystem>();
                particleSystemHolder.transform.SetParent(skillParent);
            }
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
        hotbarIndex = Mathf.Clamp(index, 0, 3);
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
        skillTomeSO.ActivateEffects(this, index);
        onActivateSkill?.Invoke(index);
    }

    public void UseConsumable()
    {
        if (!currentConsumable) return;
        if (hotbarCooldowns[hotbarIndex] > 0) return;
        currentConsumable.ActivateEffect(this);
        inventory.RemoveHotbarItemAtIndex(hotbarIndex, 1);
        hotbarCooldowns[hotbarIndex] = currentConsumable.Cooldown;
        onUseConsumable?.Invoke(hotbarIndex);

        if (inventory.GetHotbarItemAtIndex(hotbarIndex) != null) return;
        hotbar[hotbarIndex] = null;
        currentConsumable = null;
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public ParticleSystem GetParticleSystem(int index)
    {
        return particleSystems[index];
    }
}
