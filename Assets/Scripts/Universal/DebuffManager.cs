using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] float elementApplicationMultiplier = 1, bleedTickSpeed = 0.5f, bleedDuration = 7, burnTickSpeed = 0.5f, burnedDuration = 10, electrocutedTickSpeed = 0.5f, electrocutedDuration = 10, overgrownDuration = 15, dampenedDuration = 20;
    [SerializeField] int slownessMax = 1, weakenedMax = 3, crippledMax = 1, burnedMax = 1, electrocutedMax = 1, dampenedMax = 3, overgrownMax = 1, poisonedMax = 3;
    [SerializeField] Movement movementScript;
    [SerializeField] Health healthScript;
    [SerializeField] Stagger staggerScript;

    int slownessCount, weakenedCount, crippledCount, burnedCount, electrocutedCount, dampenedCount, overgrownCount, poisonedCount;

    Dictionary<Element, ElementProgress> elementProgressList;
    Dictionary<Debuff, int> debuffs;

    public Movement MovementScript => movementScript;
    public Health HealthScript => healthScript;
    public Stagger StaggerScript => staggerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elementProgressList = new ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SlowedCoroutine(float duration, float strength)
    {
        if (slownessCount >= slownessMax) yield break;
        slownessCount++;
        float change = movementScript.MovementSpeed * strength;
        movementScript.SetSpeed(movementScript.MovementSpeed - change);
        yield return new WaitForSeconds(duration);
        movementScript.SetSpeed(movementScript.MovementSpeed + change);
        slownessCount--;
    }

    IEnumerator WeakenedCoroutine(float duration, float strength)
    {
        if (weakenedCount >= weakenedMax) yield break;
        weakenedCount++;
        int change = Mathf.RoundToInt(healthScript.Defense * strength);
        healthScript.SetDefense(healthScript.Defense - change);
        yield return new WaitForSeconds(duration);
        healthScript.SetDefense(healthScript.Defense + change);
        weakenedCount--;
    }

    IEnumerator CrippledCoroutine(float duration, float strength)
    {
        if (crippledCount >= crippledMax) yield break;
        crippledCount++;
        float change = healthScript.HealingMultiplier * strength;
        healthScript.SetHealingMultiplier(healthScript.HealingMultiplier - change);
        yield return new WaitForSeconds(duration);
        healthScript.SetHealingMultiplier(healthScript.HealingMultiplier + change);
        crippledCount--;
    }

    IEnumerator BurnedCoroutine(float strength)
    {
        if (burnedCount >= burnedMax) yield break;
        burnedCount++;
        float burnEndTime = Time.time + burnedDuration;
        while (Time.time < burnEndTime)
        {
            yield return new WaitForSeconds(burnTickSpeed);
            healthScript.TakeDamage(Mathf.RoundToInt(strength), Element.Fire, transform.position - Vector3.down);
        }
        burnedCount--;
    }

    IEnumerator ElectrocutedCoroutine(float strength)
    {
        if (electrocutedCount >= electrocutedMax) yield break;
        electrocutedCount++;
        float electrocutedEndTime = Time.time + electrocutedDuration;
        while (Time.time < electrocutedEndTime)
        {
            yield return new WaitForSeconds(electrocutedTickSpeed);
            staggerScript.TakeStagger(Mathf.RoundToInt(strength), transform.position - Vector3.down);
        }
        electrocutedCount--;
    }

    IEnumerator DampenedCoroutine(float strength)
    {
        if (dampenedCount >= dampenedMax) yield break;
        dampenedCount++;
        float change = elementApplicationMultiplier * strength;
        elementApplicationMultiplier -= change;
        yield return new WaitForSeconds(dampenedDuration);
        elementApplicationMultiplier += change;
        dampenedCount--;
    }

    IEnumerator OvergrownCoroutine(float strength)
    {
        if (overgrownCount >= overgrownMax) yield break;
        overgrownCount++;
        float change = staggerScript.StaggerMultiplier * strength;
        staggerScript.SetStaggerMultiplier(staggerScript.StaggerMultiplier - change);
        yield return new WaitForSeconds(overgrownDuration);
        staggerScript.SetStaggerMultiplier(staggerScript.StaggerMultiplier + change);
        overgrownCount--;
    }

    IEnumerator BleedCoroutine(float strength)
    {
        poisonedCount++;
        float poisonEndTime = Time.time + bleedDuration;
        while (Time.time < poisonEndTime)
        {
            yield return new WaitForSeconds(bleedTickSpeed / strength);
            healthScript.TakeDamage(Mathf.RoundToInt(strength), Element.Physical, transform.position - Vector3.down);
        }
        poisonedCount--;
    }

    public void InflictElement(Element element, int amount, float strength)
    {
        if (!elementProgressList.ContainsKey(element))
        {
            elementProgressList.Add(element, new ());
        }
        ElementProgress debuff = elementProgressList[element];

        if (!debuff.IncrementAmount(amount)) return;
        switch (element)
        {
            case Element.Fire: StartCoroutine(BurnedCoroutine(strength)); break;
            case Element.Electric: StartCoroutine(ElectrocutedCoroutine(strength)); break;
            case Element.Water: StartCoroutine(DampenedCoroutine(strength)); break;
            case Element.Nature: StartCoroutine(OvergrownCoroutine(strength)); break;
            case Element.Physical: StartCoroutine(BleedCoroutine(strength)); break;
        }
    }

    public void InflictDebuff(Debuff debuff, float duration, float strength)
    {
        switch (debuff)
        {
            case Debuff.Slowed: StartCoroutine(SlowedCoroutine(duration, strength)); break;
            case Debuff.Weakened: StartCoroutine(WeakenedCoroutine(duration, strength)); break;
            case Debuff.Crippled: StartCoroutine(CrippledCoroutine(duration, strength)); break;
        }
    }

    public void RunDebuffCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}

public enum Debuff { None, Slowed, Weakened, Crippled }

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