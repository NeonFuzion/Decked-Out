using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Bleed")]
public class BleedDebuff : Debuff
{
    [SerializeField] float tickSpeed = 1;
    float TickSpeed => tickSpeed;

    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(TickSpeed / strength);
            manager.HealthScript.TakeDamage(Mathf.RoundToInt(strength), Element.Physical, manager.transform.position - Vector3.down);
        }
    }
}
