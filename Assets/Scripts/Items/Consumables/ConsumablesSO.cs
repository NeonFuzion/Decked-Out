using UnityEngine;

public abstract class ConsumablesSO : ItemSO
{
    [SerializeField] float cooldown;

    public float Cooldown { get => cooldown; }

    public abstract void ActivateEffect(HotbarManager hotbarManager);
}