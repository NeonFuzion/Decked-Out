using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] float elementApplicationMultiplier = 1;
    [SerializeField] ElementalDebuff burnedDefinition, electrocutedDefinition, dampenedDefinition, overgrownDefinition, bleedDefinition;
    [SerializeField] Movement movementScript;
    [SerializeField] Health healthScript;
    [SerializeField] Stagger staggerScript;

    DebuffBar debuffBar;
    Dictionary<Debuff, int> stackCounts;
    Dictionary<Element, ElementProgress> elementProgressList;

    public Movement MovementScript => movementScript;
    public Health HealthScript => healthScript;
    public Stagger StaggerScript => staggerScript;
    public float ElementApplicationMultiplier { get => elementApplicationMultiplier; set => elementApplicationMultiplier = value; }

    void Start()
    {
        stackCounts = new ();
        elementProgressList = new ();
    }

    void Update() { }

    IEnumerator DebuffCoroutine(Debuff debuff, float duration, float strength)
    {
        yield return StartCoroutine(debuff.DebuffCoroutine(duration, strength, this));

        int count = --stackCounts[debuff];
        debuffBar?.IncrementDebuff(debuff, count);

        if (count > 0) yield break;
        stackCounts.Remove(debuff);
    }

    public void InflictElement(Element element, int amount, float strength)
    {
        if (!elementProgressList.ContainsKey(element))
            elementProgressList.Add(element, new ());
        ElementProgress progress = elementProgressList[element];

        if (!progress.IncrementAmount(amount)) return;
        ElementalDebuff elementalDebuff = element switch 
        {
            Element.Fire => burnedDefinition,
            Element.Electric => electrocutedDefinition,
            Element.Water => dampenedDefinition,
            Element.Nature => overgrownDefinition,
            Element.Physical => bleedDefinition,
            _ => null
        };

        if (!elementalDebuff) return;
        InflictDebuff(elementalDebuff, 0, strength);
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

public class ElementProgress
{
    int progress;
    bool onCooldown;

    public bool OnCooldown => onCooldown;

    public ElementProgress()
    {
        progress = 0;
        onCooldown = false;
    }

    public bool IncrementAmount(int amount)
    {
        if (onCooldown) return false;
        progress += amount;

        if (progress < 100) return false;
        onCooldown = true;
        progress = 100;
        return true;
    }

    public void ResetProgress()
    {
        onCooldown = false;
        progress = 0;
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
