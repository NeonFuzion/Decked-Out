using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/WhirlwindTome")]
public class WhirlwindSkillSO : SkillTomeSO
{
    [SerializeField] GameObject whirlwindPrefab;
    [SerializeField] float spawnDistance = 2f;
    [SerializeField] int pullStrength = 5;

    public override void ActivateEffects(Player player, int index)
    {
        Vector2 mousePos = MainCamera.MouseWorldPosition();
        Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;
        Vector2 spawnPos = (Vector2)player.transform.position + direction * spawnDistance;

        GameObject obj = Instantiate(whirlwindPrefab, spawnPos, Quaternion.identity);
        DamageStaggerPair damagePair = DamageStaggerPairs[0];
        obj.GetComponent<DamageHolder>().Initialize(damagePair.Damage, damagePair.Stagger, -pullStrength);
    }
}
