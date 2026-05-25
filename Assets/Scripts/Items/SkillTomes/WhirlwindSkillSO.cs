using UnityEngine;

[CreateAssetMenu(menuName = "SkillTome/WhirlwindTome")]
public class WhirlwindSkillSO : SkillTomeSO
{
    [SerializeField] GameObject whirlwindPrefab;
    [SerializeField] float spawnDistance = 2f;
    [SerializeField] float radius = 2.5f;
    [SerializeField] float duration = 3f;
    [SerializeField] float tickRate = 0.5f;
    [SerializeField] float pullStrength = 5f;

    public override void ActivateEffects(Player player, int index)
    {
        Vector2 mousePos = MainCamera.MouseWorldPosition();
        Vector2 direction = (mousePos - (Vector2)player.transform.position).normalized;
        Vector2 spawnPos = (Vector2)player.transform.position + direction * spawnDistance;

        GameObject obj = Object.Instantiate(whirlwindPrefab, spawnPos, Quaternion.identity);
        Whirlwind whirlwind = obj.GetComponent<Whirlwind>();
        whirlwind.Initialize(DamageStaggerPairs[0], radius, duration, tickRate, pullStrength, Element);
    }
}
