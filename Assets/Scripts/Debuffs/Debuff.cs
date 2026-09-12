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

