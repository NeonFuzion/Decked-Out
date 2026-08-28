using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] float elementApplicationMultiplier = 1;
    [SerializeField] ElementalDebuff burnedDefinition, electrocutedDefinition, dampenedDefinition, overgrownDefinition, bleedDefinition;
    [SerializeField] DebuffBar debuffBar;
    [SerializeField] Movement movementScript;
    [SerializeField] Health healthScript;
    [SerializeField] Stagger staggerScript;

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

    public int GetStackCount(Debuff debuff)
    {
        if (!stackCounts.ContainsKey(debuff)) return 0;
        return stackCounts[debuff];
    }

    public void IncrementStackCount(Debuff debuff)
    {
        if (!stackCounts.ContainsKey(debuff)) return;
        stackCounts[debuff]++;
        if (debuffBar != null) debuffBar.AddDebuff(debuff.Sprite, stackCounts[debuff]);
    }

    public void DecrementStackCount(Debuff debuff)
    {
        if (!stackCounts.ContainsKey(debuff)) return;
        stackCounts[debuff]--;
        if (debuffBar != null) debuffBar.RemoveDebuff(debuff.Sprite, stackCounts[debuff]);
    }

    public void InflictElement(Element element, int amount, float strength)
    {
        if (!elementProgressList.ContainsKey(element))
            elementProgressList.Add(element, new ());

        ElementProgress progress = elementProgressList[element];
        if (!progress.IncrementAmount(amount)) return;

        switch (element)
        {
            case Element.Fire: burnedDefinition.TriggerDebuff(strength, this); break;
            case Element.Electric: electrocutedDefinition.TriggerDebuff(strength, this); break;
            case Element.Water: dampenedDefinition.TriggerDebuff(strength, this); break;
            case Element.Nature: overgrownDefinition.TriggerDebuff(strength, this); break;
            case Element.Physical: bleedDefinition.TriggerDebuff(strength, this); break;
        }
    }

    public void InflictDebuff(Debuff debuff, float duration, float strength)
    {
        debuff.TriggerDebuff(duration, strength, this);
    }

    public void RunDebuffCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
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
