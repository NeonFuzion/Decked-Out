using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment/Armor")]
public class Armor : Equipment
{
    [SerializeField] float defense;
    [SerializeField] StatBoost secondaryStat;
    [SerializeField] ArmorPiece armorPiece;
    [SerializeField] SetBonus setBonus;

    StatBoost[] substats;

    public float Defense { get => defense; }
    public StatBoost SecondaryStat { get => secondaryStat; }
    public ArmorPiece ArmorPiece { get => armorPiece; }
    public SetBonus SetBonus { get => setBonus; }
    public StatBoost[] Substats { get => substats; }
}

public enum ArmorPiece { Helmet, Chestplate, Leggings, Boots }