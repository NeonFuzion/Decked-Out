using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Dampened")]
public class DampenedDebuff : ElementalDebuff
{
    public override void TriggerDebuff(float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(strength, manager));

    IEnumerator Coroutine(float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        float change = manager.ElementApplicationMultiplier * strength;
        manager.ElementApplicationMultiplier -= change;
        yield return new WaitForSeconds(Duration);
        manager.ElementApplicationMultiplier += change;
        manager.DecrementStackCount(this);
    }
}
