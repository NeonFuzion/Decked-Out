using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Overgrown")]
public class OvergrownDebuff : ElementalDebuff
{
    public override IEnumerator ElementalDebuffCoroutine(float strength, DebuffManager manager)
    {
        float change = manager.StaggerScript.StaggerMultiplier * strength;
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier - change);
        yield return new WaitForSeconds(Duration);
        manager.StaggerScript.SetStaggerMultiplier(manager.StaggerScript.StaggerMultiplier + change);
    }
}
