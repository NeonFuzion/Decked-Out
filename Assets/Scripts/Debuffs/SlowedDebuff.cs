using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Slowed")]
public class SlowedDebuff : Debuff
{
    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        float change = manager.MovementScript.MovementSpeed * strength;
        manager.MovementScript.SetSpeed(manager.MovementScript.MovementSpeed - change);
        yield return new WaitForSeconds(duration);
        manager.MovementScript.SetSpeed(manager.MovementScript.MovementSpeed + change);
    }
}
