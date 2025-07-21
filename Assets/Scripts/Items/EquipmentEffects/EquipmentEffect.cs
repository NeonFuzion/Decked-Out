using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EquipmentEffect : ScriptableObject
{
    [SerializeField] string abilityName, description;

    protected GameObject player;
    protected EquipmentEffectsManager equipmentEffectsManager;

    public string Name { get => abilityName; }
    public string Description { get => description; }

    public virtual void Instantiate(GameObject player)
    {
        this.player = player;
        equipmentEffectsManager = player.GetComponent<EquipmentEffectsManager>();
    }
}