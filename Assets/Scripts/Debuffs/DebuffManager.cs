using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] float elementApplicationMultiplier = 1;
    [SerializeField] Debuff burnedDefinition, electrocutedDefinition, dampenedDefinition, overgrownDefinition, bleedDefinition;
    [SerializeField] Movement movementScript;
    [SerializeField] Health healthScript;
    [SerializeField] Stagger staggerScript;

    DebuffBar debuffBar;
    Dictionary<Debuff, int> stackCounts;

    public Movement MovementScript => movementScript;
    public Health HealthScript => healthScript;
    public Stagger StaggerScript => staggerScript;
    public float ElementApplicationMultiplier { get => elementApplicationMultiplier; set => elementApplicationMultiplier = value; }

    void Start()
    {
        stackCounts = new ();
    }

    void Update() { }

    IEnumerator DebuffCoroutine(Debuff debuff, float duration, float strength)
    {
        debuffBar?.IncrementDebuff(debuff, ++stackCounts[debuff]);
        yield return StartCoroutine(debuff.DebuffCoroutine(duration, strength, this));
        debuffBar?.IncrementDebuff(debuff, --stackCounts[debuff]);

        if (stackCounts[debuff] > 0) yield break;
        stackCounts.Remove(debuff);
    }

    public void InflictDebuff(Debuff debuff, float duration, float strength)
    {
        if (stackCounts.ContainsKey(debuff))
        {
            if (stackCounts[debuff] >= debuff.MaxStackCount) return;
            stackCounts[debuff]++;
        }
        else
        {
            stackCounts.Add(debuff, 1);
        }
        StartCoroutine(DebuffCoroutine(debuff, duration, strength));
    }

    public void SetDebuffBar(DebuffBar debuffBar)
    {
        this.debuffBar = debuffBar;
    }
}

[Serializable]
public struct DebuffData
{
    [SerializeField] float duration, strength;
    [SerializeField] Debuff debuff;

    public DebuffData(float duration, float strength, Debuff debuff)
    {
        this.duration = duration;
        this.strength = strength;
        this.debuff = debuff;
    }

    public float Duration => duration;
    public float Strength => strength;
    public Debuff Debuff => debuff;
}
