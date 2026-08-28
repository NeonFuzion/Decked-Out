using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Slowed")]
public class SlowedDebuff : Debuff
{
    public override void TriggerDebuff(float duration, float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(duration, strength, manager));

    IEnumerator Coroutine(float duration, float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        float change = manager.MovementScript.MovementSpeed * strength;
        manager.MovementScript.SetSpeed(manager.MovementScript.MovementSpeed - change);
        yield return new WaitForSeconds(duration);
        manager.MovementScript.SetSpeed(manager.MovementScript.MovementSpeed + change);
        manager.DecrementStackCount(this);
    }
}
