using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EquipmentEffect/HealthRegen")]
public class HealthRegen : EquipmentEffect
{
    [SerializeField] float regenerationRate, intervalDuration;

    Health health;

    public override void Instantiate(GameObject player)
    {
        base.Instantiate(player);

        health = player.GetComponent<Health>();
        equipmentEffectsManager.AddTimerPair(RegenerateHealth, intervalDuration, this);
    }

    public void RegenerateHealth()
    {
        health.Heal(Mathf.RoundToInt(health.MaxHP * regenerationRate));
        equipmentEffectsManager.AddTimerPair(RegenerateHealth, intervalDuration, this);
    }
}