using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/BuffStat")]
public class StatBuffConsumableSO : ConsumablesSO
{
    [SerializeField] PlayerStat playerStat;
    [SerializeField] BoostType boostType;
    [SerializeField] int amount, duration;

    public override void ActivateEffect(HotbarManager hotbarManager)
    {
        Player player = hotbarManager.GetComponent<Player>();
        player.AddTemporaryBuff(playerStat, amount, duration, boostType);
    }
}
