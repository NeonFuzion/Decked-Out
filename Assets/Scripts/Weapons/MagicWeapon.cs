using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon/MagicWeapon")]
public class MagicWeapon : Weapon
{
    [SerializeField] int projectileCount, projectileSpread;
    [SerializeField] GameObject prefabProjectile;
    [SerializeField] ProjectileData projectileData;

    public int ProjectileCount { get => projectileCount; }
    public int ProjectileSpread { get => projectileSpread; }
    public GameObject PrefabProjectile { get => prefabProjectile; }
    public ProjectileData ProjectileData { get => projectileData; }

    public override void AttackActionHandle(int attackIndex, Transform transform, Vector2 mousePosition)
    {
        int range = projectileSpread;
        Vector2 offset = Vector2.zero;
        Shooter script = transform.GetComponent<Shooter>();

        for (int i = 0; i < projectileCount; i++)
        {
            if (projectileCount > 1)
                offset = new Vector2((Random.value * 2) - 1, (Random.value * 2) - 1) * range;
            if (projectileData.MaxHeight == 0)
                offset = (mousePosition - (Vector2)transform.position).normalized * 100 + offset * 50;
            script.FireProjectile(projectileData, ProjectileTargetType.Friendly, mousePosition + offset, Element, GetMultipliersByIndex(attackIndex));
        }
    }

    public override void AttackAnimationHandle(int animationIndex, Transform transform)
    {

    }
}
