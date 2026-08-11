using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [SerializeField] int slownessMax, weakenedMax, crippledMax, burnedMax, electrocutedMax, dampenedMax, overgrownMax, poisonedMax;
    [SerializeField] Movement movementScript;
    [SerializeField] Health health;

    Dictionary<Element, ElementalDebuff> debuffs;

    int slownessCount, weakenedCount, crippledCount, burnedCount, electrocutedCount, dampenedCount, overgrownCount, poisonedCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        debuffs = new ();
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
        health.SetDefense(health.Defense - change);;
        yield return new WaitForSeconds(duration);
        health.SetDefense(health.Defense + change);
    }

    IEnumerator CrippledCoroutine(float duration, float strength)
    {
        float change = health.HealingMultiplier * strength;
        health.SetHealingMultiplier(health.HealingMultiplier - change);
        yield return new WaitForSeconds(duration);
        health.SetHealingMultiplier(health.HealingMultiplier + strength);
    }

    public void InflictElement(Element element, int amount)
    {
        if (!debuffs.ContainsKey(element))
        {
            debuffs.Add(element, new ());
        }
        ElementalDebuff debuff = debuffs[element];

        if (!debuff.IncrementAmount(amount)) return;
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
