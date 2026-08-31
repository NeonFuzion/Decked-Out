using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuffs/Electrocuted")]
public class ElectrocutedDebuff : ElementalDebuff
{
    [SerializeField] float tickSpeed = 1;
    float TickSpeed => tickSpeed;

    public override IEnumerator ElementalDebuffCoroutine(float strength, DebuffManager manager)
    {
        float endTime = Time.time + Duration;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(TickSpeed);
            manager.StaggerScript.TakeStagger(Mathf.RoundToInt(strength), manager.transform.position - Vector3.down);
        }
    }
}
