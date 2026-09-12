using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Dampened")]
public class DampenedDebuff : Debuff
{
    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        float change = manager.ElementApplicationMultiplier * strength;
        manager.ElementApplicationMultiplier -= change;
        yield return new WaitForSeconds(duration);
        manager.ElementApplicationMultiplier += change;
    }
}
