using UnityEngine;

[CreateAssetMenu(fileName = "Vampire", menuName = "EquipmentEffect/Vampire")]
public class VampireSO : PassiveEffectSO
{
    [SerializeField] float healPercentage;
    [SerializeField] float cooldown;

    public float HealPercentage { get => healPercentage; }
    public float Cooldown { get => cooldown; }

    public override PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        Vampire vampire = new (this, player, equipmentEffectsManager);
        return vampire;
    }
}

public class Vampire : PassiveEffect
{
    VampireSO data;
    Health health;
    EffectState effectState;

    public Vampire(VampireSO vampireSO, GameObject player, EquipmentEffectsManager equipmentEffectsManager) : base (player, equipmentEffectsManager)
    {
        data = vampireSO;

        effectState = EffectState.Idle;
        health = player.GetComponent<Health>();
        equipmentEffectsManager.OnKill.AddListener(ActivateEffect);
    }

    public void ActivateEffect()
    {
        if (effectState != EffectState.Idle) return;
        effectState = EffectState.Cooldown;
        health.Heal(Mathf.RoundToInt(health.MaxHP * data.HealPercentage));
        equipmentEffectsManager.AddTimerPair(FinishCooldown, data.Cooldown, this);
    }

    public void FinishCooldown()
    {
        effectState = EffectState.Idle;
    }

    enum EffectState { Idle, Cooldown }
}