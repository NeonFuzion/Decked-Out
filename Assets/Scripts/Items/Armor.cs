using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment/Armor")]
public class Armor : Equipment
{
    [SerializeField] float defense;
    [SerializeField] StatBoost secondaryStat;
    [SerializeField] ArmorPiece armorPiece;

    StatBoost[] substats;

    public float Defense { get => defense; }
    public StatBoost SecondaryStat { get => secondaryStat; }
    public ArmorPiece ArmorPiece { get => armorPiece; }
    public StatBoost[] Substats { get => substats; }
}

public enum ArmorPiece { Helmet, Chestplate, Leggings, Boots }