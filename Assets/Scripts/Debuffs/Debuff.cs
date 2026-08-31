using System.Collections;
using UnityEngine;

public abstract class Debuff : ScriptableObject
{
    [SerializeField] int maxStackCount = 1;
    [SerializeField] Sprite sprite;
    public int MaxStackCount => maxStackCount;
    public Sprite Sprite => sprite;
    public abstract IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager);
}

public abstract class ElementalDebuff : Debuff
{
    [SerializeField] float duration = 10;
    
    public float Duration => duration;

    public override IEnumerator DebuffCoroutine(float duration, float strength, DebuffManager manager)
        => ElementalDebuffCoroutine(strength, manager);

    public abstract IEnumerator ElementalDebuffCoroutine(float strength, DebuffManager manager);
}
