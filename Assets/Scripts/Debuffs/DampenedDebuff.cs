using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Dampened")]
public class DampenedDebuff : ElementalDebuff
{
    public override IEnumerator ElementalDebuffCoroutine(float strength, DebuffManager manager)
    {
        float change = manager.ElementApplicationMultiplier * strength;
        manager.ElementApplicationMultiplier -= change;
        yield return new WaitForSeconds(Duration);
        manager.ElementApplicationMultiplier += change;
    }
}
