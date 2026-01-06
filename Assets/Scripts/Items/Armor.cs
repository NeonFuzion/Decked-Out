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

    public void AddSubstat(PlayerStat stat = PlayerStat.None)
    {
        float boostAmount = 0;
        switch (stat)
        {
            case PlayerStat.Attack: boostAmount = Random.Range(4, 7); break;
            case PlayerStat.Defense: boostAmount = Random.Range(4, 7); break;
            case PlayerStat.Health: boostAmount = Random.Range(4, 7); break;
            case PlayerStat.ManaRegeneration: boostAmount = Random.Range(3, 5); break;

        }
    }
}

public enum ArmorPiece { Helmet, Chestplate, Leggings, Boots }