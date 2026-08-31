using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Weakened")]
public class WeakenedDebuff : Debuff
{
    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        int change = Mathf.RoundToInt(manager.HealthScript.Defense * strength);
        manager.HealthScript.SetDefense(manager.HealthScript.Defense - change);
        yield return new WaitForSeconds(duration);
        manager.HealthScript.SetDefense(manager.HealthScript.Defense + change);
    }
}
