using UnityEngine;

[CreateAssetMenu(fileName = "Berserker", menuName = "EquipmentEffect/Berserker")]
public class BerserkerSO : PassiveEffectSO
{
    [SerializeField] float attackBoost;
    [SerializeField] float duration, cooldown;

    public float AttackBoost { get => attackBoost; }
    public float Duration { get => duration; }
    public float Cooldown { get => cooldown; }

    public override PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        Berserker berserker = new (this, player, equipmentEffectsManager);
        return berserker;
    }
}

public class Berserker : PassiveEffect
{
    BerserkerSO data;
    Player playerScript;
    EffectState effectState;

    public Berserker(BerserkerSO berserkerSO, GameObject player, EquipmentEffectsManager equipmentEffectsManager) : base (player, equipmentEffectsManager)
    {
        data = berserkerSO;

        effectState = EffectState.Idle;
        playerScript = player.GetComponent<Player>();
        equipmentEffectsManager.OnKill.AddListener(ActivateEffect);
    }

    public void ActivateEffect()
    {
        if (effectState != EffectState.Idle) return;
        playerScript.IncrementStat(PlayerStat.Attack, data.AttackBoost, BoostType.Percentage);
        equipmentEffectsManager.AddTimerPair(DeactivateEffect, data.Duration, this);
    }

    public void DeactivateEffect()
    {
        if (effectState != EffectState.Active) return;
        playerScript.IncrementStat(PlayerStat.Attack, -data.AttackBoost, BoostType.Percentage);
        equipmentEffectsManager.AddTimerPair(DeactivateEffect, data.Cooldown, this);
    }

    enum EffectState { Idle, Active, Cooldown }
}