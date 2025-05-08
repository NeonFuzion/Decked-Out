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

    public override void AttackActionHandle(int damage, bool isCrit, Transform transform)
    {
        int range = projectileSpread;
        Vector2 offset = Vector2.zero;
        Vector2 mousePos = MainCamera.MousePosition;
        Shooter script = transform.GetChild(0).GetChild(0).GetComponentInChildren<Shooter>();
        
        for (int i = 0; i < projectileCount; i++)
        {
            if (projectileCount > 1)
                offset = new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            script.FireProjectile(projectileData, ProjectileTargetType.Friendly, mousePos + offset, Element, damage, isCrit);
        }
    }

    public override void AttackAnimationHandle(int animationIndex, Transform transform)
    {

    }
}
