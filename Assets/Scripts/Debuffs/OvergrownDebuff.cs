using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Overgrown")]
public class OvergrownDebuff : Debuff
{

    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        float change = manager.StaggerScript.StaggerMultiplier * strength;
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier - change);
        yield return new WaitForSeconds(duration);
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier + change);
    }
}
