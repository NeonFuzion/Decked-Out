using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "SkillTome/Lingering")]
public class LingeringSkillSO : SkillTomeSO
{
    [SerializeField] GameObject lingerPrefab;
    [SerializeField] float spawnDistance = 2f, radius = 2.5f, tickInterval = 0.5f;
    [SerializeField] int knockBack = -5, tickCount = 6;

    public override void ActivateEffects(Player player, int index)
    {
        Vector2 mousePos = MainCamera.MouseWorldPosition();
        Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;
        Vector2 spawnPos = (Vector2)player.transform.position + direction * spawnDistance;

        GameObject obj = Instantiate(lingerPrefab, spawnPos, Quaternion.identity);
        DamageStaggerPair damagePair = DamageStaggerPairs[0];

        Timer timer = obj.GetComponent<Timer>();
        timer.SetTimer(tickInterval);

        MultiTrigger multiTrigger = obj.GetComponent<MultiTrigger>();
        multiTrigger.SetTriggerCount(tickCount);
        multiTrigger.OnTrigger.AddListener(() => {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(obj.transform.position, radius).Where(collider => collider.gameObject != obj).ToArray();
            DamageStaggerPair damageStaggerPair = DamageStaggerPairs[0];
            AttackData attackData = new (Element, obj.transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger);
            EventManager.InvokeOnEnemyDataAcquired(colliders, attackData);
        });
        multiTrigger.OnFinish.AddListener(() => Destroy(obj));
    }
}
