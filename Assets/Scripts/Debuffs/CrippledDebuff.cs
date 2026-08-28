using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Crippled")]
public class CrippledDebuff : Debuff
{
    public override void TriggerDebuff(float duration, float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(duration, strength, manager));

    IEnumerator Coroutine(float duration, float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        float change = manager.HealthScript.HealingMultiplier * strength;
        manager.HealthScript.SetHealingMultiplier(manager.HealthScript.HealingMultiplier - change);
        yield return new WaitForSeconds(duration);
        manager.HealthScript.SetHealingMultiplier(manager.HealthScript.HealingMultiplier + change);
        manager.DecrementStackCount(this);
    }
}
