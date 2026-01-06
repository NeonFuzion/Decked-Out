using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EquipmentEffect/HealthRegen")]

public class HealthRegenSO : PassiveEffectSO
{
    [SerializeField] float regenerationRate, intervalDuration;
    
    public float RegenerationRate { get => regenerationRate; }
    public float IntervalDuration { get => intervalDuration; }

    public override PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        HealthRegen healthRegen = new (this, player, equipmentEffectsManager);
        return healthRegen;
    }
}

public class HealthRegen : PassiveEffect
{
    HealthRegenSO data;
    Health health;

    public HealthRegen(HealthRegenSO healthRegenSO, GameObject player, EquipmentEffectsManager equipmentEffectsManager) : base (player, equipmentEffectsManager)
    {
        data = healthRegenSO;
        health = player.GetComponent<Health>();

        equipmentEffectsManager.AddTimerPair(RegenerateHealth, data.IntervalDuration, this);
    }

    public void RegenerateHealth()
    {
        health.Heal(Mathf.RoundToInt(health.MaxHP * data.RegenerationRate));
        equipmentEffectsManager.AddTimerPair(RegenerateHealth, data.IntervalDuration, this);
    }
}