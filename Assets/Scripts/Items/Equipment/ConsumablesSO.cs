using UnityEngine;

public abstract class ConsumablesSO : EquipmentSO
{
    [SerializeField] float cooldown;

    public float Cooldown { get => cooldown; }

    public abstract void ActivateEffect(Player player);
}