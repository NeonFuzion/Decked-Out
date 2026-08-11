using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] float burnTickSpeed = 0.5f, elementApplicationMultiplier = 1, electrocutedTickSpeed = 0.5f, overgrownStaggerMultiplier = 0.4f, poisonTickSpeed = 0.5f;
    [SerializeField] int slownessMax = 1, weakenedMax = 3, crippledMax = 1, burnedMax = 1, electrocutedMax = 1, dampenedMax = 3, overgrownMax = 1, poisonedMax = 3;
    [SerializeField] Movement movementScript;
    [SerializeField] Health health;
    [SerializeField] Stagger stagger;

    Dictionary<Element, ElementalDebuff> elementProgressList;
    Dictionary<Debuff, int> debuffs;

    int slownessCount, weakenedCount, crippledCount, burnedCount, electrocutedCount, dampenedCount, overgrownCount, poisonedCount;

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
        float change = movementScript.MovementSpeed * strength;
        movementScript.SetSpeed(movementScript.MovementSpeed - change);
        yield return new WaitForSeconds(duration);
        movementScript.SetSpeed(movementScript.MovementSpeed + change);
    }

    IEnumerator WeakenedCoroutine(float duration, float strength)
    {
        int change = Mathf.RoundToInt(health.Defense * strength);
        health.SetDefense(health.Defense - change);
        yield return new WaitForSeconds(duration);
        health.SetDefense(health.Defense + change);
    }

    IEnumerator CrippledCoroutine(float duration, float strength)
    {
        float change = health.HealingMultiplier * strength;
        health.SetHealingMultiplier(health.HealingMultiplier - change);
        yield return new WaitForSeconds(duration);
        health.SetHealingMultiplier(health.HealingMultiplier + change);
    }

    IEnumerator BurnedCoroutine(float duration, float strength)
    {
        float burnEndTime = Time.time + duration;
        while (Time.time < burnEndTime)
        {
            yield return new WaitForSeconds(burnTickSpeed);
            health.TakeDamage(Mathf.RoundToInt(strength), Element.Electric, transform.position - Vector3.down);
        }
    }

    IEnumerator ElectrocutedCoroutine(float duration, float strength)
    {
        float electrocutedEndTime = Time.time + duration;
        while (Time.time < electrocutedEndTime)
        {
            yield return new WaitForSeconds(electrocutedTickSpeed);
            stagger.TakeStagger(Mathf.RoundToInt(strength), transform.position - Vector3.down);
        }
    }

    IEnumerator DampenedCoroutine(float duration, float strength)
    {
        float change = elementApplicationMultiplier * strength;
        elementApplicationMultiplier -= change;
        yield return new WaitForSeconds(duration);
        elementApplicationMultiplier += change;
    }

    IEnumerator OvergrownCoroutine(float duration, float strength)
    {
        float change = stagger.StaggerMultiplier * strength;
        stagger.SetStaggerMultiplier(stagger.StaggerMultiplier - change);
        yield return new WaitForSeconds(duration);
        stagger.SetStaggerMultiplier(stagger.StaggerMultiplier + change);
    }

    IEnumerator PoisonedCoroutine(float duration, float strength)
    {
        float poisonEndTime = Time.time + duration;
        while (Time.time < poisonEndTime)
        {
            yield return new WaitForSeconds(poisonTickSpeed / strength);
            health.TakeDamage(Mathf.RoundToInt(strength), Element.Electric, transform.position - Vector3.down);
        }
    }

    public void InflictElement(Element element, int amount)
    {
        if (!elementProgressList.ContainsKey(element))
        {
            elementProgressList.Add(element, new ());
        }
        ElementalDebuff debuff = elementProgressList[element];

        if (!debuff.IncrementAmount(amount)) return;
    }

    public void InflictDebuff(Debuff debuff, float duration, float strength)
    {
        switch (debuff)
        {
            case Debuff.Slowed: StartCoroutine(SlowedCoroutine(duration, strength)); break;
            case Debuff.Weakened: StartCoroutine(WeakenedCoroutine(duration, strength)); break;
            case Debuff.Crippled: StartCoroutine(CrippledCoroutine(duration, strength)); break;
            case Debuff.Burned: StartCoroutine(BurnedCoroutine(duration, strength)); break;
            case Debuff.Electrocuted: StartCoroutine(ElectrocutedCoroutine(duration, strength)); break;
            case Debuff.Dampened: StartCoroutine(DampenedCoroutine(duration, strength)); break;
            case Debuff.Overgrown: StartCoroutine(OvergrownCoroutine(duration, strength)); break;
        }
    }
}

public enum Debuff { None, Slowed, Weakened, Crippled, Burned, Electrocuted, Dampened, Overgrown, Poisoned }

public class ElementalDebuff
{
    int progress;
    bool onCooldown;

    public bool OnCooldown => onCooldown;

    public ElementalDebuff()
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
