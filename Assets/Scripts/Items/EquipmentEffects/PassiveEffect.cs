using UnityEngine;

public abstract class PassiveEffectSO : ScriptableObject
{
    [SerializeField] string abilityName, description;

    public string Name { get => abilityName; }
    public string Description { get => description; }

    public abstract PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager);
}

public abstract class PassiveEffect
{
    protected GameObject player;
    protected EquipmentEffectsManager equipmentEffectsManager;

    public PassiveEffect(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        this.player = player;
        this.equipmentEffectsManager = equipmentEffectsManager;
    }
}