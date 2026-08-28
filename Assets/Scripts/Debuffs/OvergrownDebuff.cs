using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Overgrown")]
public class OvergrownDebuff : ElementalDebuff
{
    public override void TriggerDebuff(float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(strength, manager));

    IEnumerator Coroutine(float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        float change = manager.StaggerScript.StaggerMultiplier * strength;
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier - change);
        yield return new WaitForSeconds(Duration);
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier + change);
        manager.DecrementStackCount(this);
    }
}
