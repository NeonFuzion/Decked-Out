using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Weakened")]
public class WeakenedDebuff : Debuff
{
    public override void TriggerDebuff(float duration, float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(duration, strength, manager));

    IEnumerator Coroutine(float duration, float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        int change = Mathf.RoundToInt(manager.HealthScript.Defense * strength);
        manager.HealthScript.SetDefense(manager.HealthScript.Defense - change);
        yield return new WaitForSeconds(duration);
        manager.HealthScript.SetDefense(manager.HealthScript.Defense + change);
        manager.DecrementStackCount(this);
    }
}
