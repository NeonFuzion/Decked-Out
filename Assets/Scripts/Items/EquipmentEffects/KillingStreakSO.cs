using UnityEngine;

[CreateAssetMenu(fileName = "KillingStreak", menuName = "EquipmentEffect/KillingStreak")]
public class KillingStreakSO : PassiveEffectSO
{
    [SerializeField] float attackBoost;

    public float AttackBoost { get => attackBoost; }

    public override PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        KillingStreak killingStreak = new (this, player, equipmentEffectsManager);
        return killingStreak;
    }
}

public class KillingStreak : PassiveEffect
{
    KillingStreakSO data;
    Player playerScript;
    EffectState effectState;

    public KillingStreak(KillingStreakSO killingStreakSO, GameObject player, EquipmentEffectsManager equipmentEffectsManager) : base (player, equipmentEffectsManager)
    {
        data = killingStreakSO;

        effectState = EffectState.Idle;
        playerScript = player.GetComponent<Player>();
        equipmentEffectsManager.OnKill.AddListener(ActivateEffect);
        equipmentEffectsManager.OnDamageDealt.AddListener(DeactivateEffect);
    }

    void ActivateEffect()
    {
        if (effectState != EffectState.Idle) return;
        effectState = EffectState.Active;
        playerScript.IncrementStat(PlayerStat.Attack, data.AttackBoost, BoostType.Percentage);
    }

    void DeactivateEffect()
    {
        if (effectState != EffectState.Active) return;
        effectState = EffectState.Idle;
        playerScript.IncrementStat(PlayerStat.Attack, -data.AttackBoost, BoostType.Percentage);
    }

    enum EffectState { Idle, Active, Cooldown }
}