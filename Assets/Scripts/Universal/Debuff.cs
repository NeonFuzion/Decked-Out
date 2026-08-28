using UnityEngine;

public abstract class Debuff : ScriptableObject
{
    [SerializeField] int maxStackCount = 1;
    [SerializeField] Sprite sprite;
    public int MaxStackCount => maxStackCount;
    public Sprite Sprite => sprite;
    public abstract void TriggerDebuff(float duration, float strength, DebuffManager manager);
}

public abstract class ElementalDebuff : Debuff
{
    [SerializeField] float duration = 10;
    
    public float Duration => duration;

    public override void TriggerDebuff(float duration, float strength, DebuffManager manager)
        => TriggerDebuff(strength, manager);

    public abstract void TriggerDebuff(float strength, DebuffManager manager);
}
