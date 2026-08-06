using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable")]
public class PassiveHealing : ConsumablesSO
{
    [SerializeField] float tickCooldown;
    [SerializeField] int tickCount, healAmount;

    Health healthScript;
    HotbarManager hotbarManager;

    public override void ActivateEffect(HotbarManager hotbarManager)
    {
        this.hotbarManager = hotbarManager;

        healthScript = hotbarManager.Player.GetComponent<Health>();
        hotbarManager.RunCoroutine(HealCoroutine(tickCount));
    }

    IEnumerator HealCoroutine(int remainingTicks)
    {
        yield return new WaitForSeconds(tickCooldown);
        healthScript.Heal(healAmount);
        
        if (remainingTicks == 0) yield break;
        hotbarManager.RunCoroutine(HealCoroutine(remainingTicks - 1));
    }
}
