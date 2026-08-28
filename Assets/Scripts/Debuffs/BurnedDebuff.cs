using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Burned")]
public class BurnedDebuff : ElementalDebuff
{
    [SerializeField] float tickSpeed = 1;
    float TickSpeed => tickSpeed;

    public override void TriggerDebuff(float strength, DebuffManager manager)
        => manager.RunDebuffCoroutine(Coroutine(strength, manager));

    IEnumerator Coroutine(float strength, DebuffManager manager)
    {
        if (manager.GetStackCount(this) >= MaxStackCount) yield break;
        manager.IncrementStackCount(this);
        float endTime = Time.time + Duration;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(TickSpeed);
            manager.HealthScript.TakeDamage(Mathf.RoundToInt(strength), Element.Fire, manager.transform.position - Vector3.down);
        }
        manager.DecrementStackCount(this);
    }
}
