using UnityEngine;
using System.Linq;
using System.Collections;

[CreateAssetMenu(menuName = "SkillTome/Lingering")]
public class LingeringSkillSO : SkillTomeSO
{
    [SerializeField] float spawnDistance = 2f, radius = 2.5f, tickInterval = 0.5f;
    [SerializeField] int knockBack = -5, tickCount = 6;

    public override void ActivateEffects(HotbarManager hotbarManager, int index)
    {
        Vector2 mousePos = MainCamera.MouseWorldPosition();
        Vector2 direction = (mousePos - (Vector2)hotbarManager.transform.position).normalized;
        Vector2 spawnPos = (Vector2)hotbarManager.transform.position + direction * spawnDistance;

        ParticleSystem particleSystem = hotbarManager.GetParticleSystem(index);
        GameObject obj = particleSystem.gameObject;
        obj.transform.SetParent(null);
        obj.transform.position = spawnPos;
        particleSystem.Play();
        DamageStaggerPair damagePair = DamageStaggerPairs[0];

        Timer timer = obj.GetComponent<Timer>();
        timer.SetTimer(tickInterval);

        MultiTrigger multiTrigger = obj.GetComponent<MultiTrigger>();
        multiTrigger.Initialize(tickCount);
        multiTrigger.OnTrigger.AddListener(() => {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(obj.transform.position, radius).Where(collider => collider.gameObject != obj).ToArray();
            DamageStaggerPair damageStaggerPair = DamageStaggerPairs[0];
            AttackData attackData = new (Element, obj.transform.position, damageStaggerPair.Damage, damageStaggerPair.Stagger, knockBack);
            EventManager.InvokeOnEnemyDataAcquired(colliders, attackData);
        });

        hotbarManager.RunCoroutine(VisualLagCoroutine(particleSystem, obj.transform, hotbarManager.SkillParent));
    }

    IEnumerator VisualLagCoroutine(ParticleSystem particleSystem, Transform visual, Transform parent)
    {
        yield return new WaitWhile(() => particleSystem.isPlaying);
        visual.SetParent(parent);
    }
}
