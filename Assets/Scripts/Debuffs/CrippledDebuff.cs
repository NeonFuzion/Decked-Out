using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Crippled")]
public class CrippledDebuff : Debuff
{
    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        float change = manager.HealthScript.HealingMultiplier * strength;
        manager.HealthScript.SetHealingMultiplier(manager.HealthScript.HealingMultiplier - change);
        yield return new WaitForSeconds(duration);
        manager.HealthScript.SetHealingMultiplier(manager.HealthScript.HealingMultiplier + change);
    }
}
