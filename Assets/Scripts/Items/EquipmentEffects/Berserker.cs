using UnityEngine;

[CreateAssetMenu(fileName = "Berserker", menuName = "EquipmentEffect/Berserker")]
public class Berserker : EquipmentEffect
{
    [SerializeField] int criticalChanceIncrease;
    [SerializeField] float duration, cooldown;

    Player playerScript;
    EffectState effectState;

    public override void Instantiate(GameObject player)
    {
        base.Instantiate(player);

        effectState = EffectState.Idle;
        playerScript = player.GetComponent<Player>();
        equipmentEffectsManager.OnKill.AddListener(ActivateEffect);
    }

    public void ActivateEffect()
    {
        if (effectState != EffectState.Idle) return;
        playerScript.IncrementStat(PlayerStat.CriticalChance, criticalChanceIncrease);
        equipmentEffectsManager.AddTimerPair(DeactivateEffect, duration, this);
    }

    public void DeactivateEffect()
    {
        if (effectState != EffectState.Active) return;
        playerScript.IncrementStat(PlayerStat.CriticalChance, -criticalChanceIncrease);
        equipmentEffectsManager.AddTimerPair(DeactivateEffect, cooldown, this);
    }

    enum EffectState { Idle, Active, Cooldown }
}
