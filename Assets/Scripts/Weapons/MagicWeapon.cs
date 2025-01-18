using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon/MagicWeapon")]
public class MagicWeapon : Weapon
{
    [SerializeField] GameObject prefabProjectile;
    [SerializeField] ProjectileData projectileData;

    public GameObject PrefabProjectile { get => prefabProjectile; }
    public ProjectileData ProjectileData { get => projectileData; }
}
