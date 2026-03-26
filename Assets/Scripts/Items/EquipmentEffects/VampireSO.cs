using UnityEngine;

[CreateAssetMenu(fileName = "Vampire", menuName = "EquipmentEffect/Vampire")]
public class VampireSO : PassiveEffectSO
{
    [SerializeField] float healPercentage;

    public float HealPercentage { get => healPercentage; }

    public override PassiveEffect Initialize(GameObject player, EquipmentEffectsManager equipmentEffectsManager)
    {
        Vampire vampire = new (this, player, equipmentEffectsManager);
        return vampire;
    }
}

public class Vampire : PassiveEffect
{
    VampireSO data;
    Health health;

    public Vampire(VampireSO vampireSO, GameObject player, EquipmentEffectsManager equipmentEffectsManager) : base (player, equipmentEffectsManager)
    {
        data = vampireSO;

        health = player.GetComponent<Health>();
        equipmentEffectsManager.OnKill.AddListener(ActivateEffect);
    }

    public void ActivateEffect()
    {
        health.Heal(Mathf.RoundToInt(health.MaxHP * data.HealPercentage));
    }
}