using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment/Armor")]
public class Armor : Equipment
{
    [SerializeField] float defense;
    [SerializeField] ArmorPiece armorPiece;
    [SerializeField] PassiveEffectSO passiveEffectSO;
    [SerializeField] StatBoost[] substats;

    public float Defense { get => defense; }
    public ArmorPiece ArmorPiece { get => armorPiece; }
    public PassiveEffectSO PassiveEffectSO { get => passiveEffectSO; }
    public StatBoost[] Substats { get => substats; }
}

public enum ArmorPiece { Helmet, Chestplate, Leggings, Boots }