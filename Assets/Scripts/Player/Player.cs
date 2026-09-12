using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : Being
{
    [SerializeField] float baseResourceRegen, perfectDodgeResourceGain, perfectDodgeTimeScale, perfectDodgeDuration, damageConstant = 20;
    [SerializeField] AnimationCurve slowCurve;
    [SerializeField] Transform sprite, dashCount;
    [SerializeField] UnityEvent<float> onManaChanged;
    [SerializeField] UnityEvent<int, bool> onDamageInflicted;

    Dictionary<PlayerStat, float> resetStats, baseStats, percentageStats, flatStats;
    List<StatBoost> temporaryPercentBuffs, temporaryFlatBuffs;

    int baseSpeed, curSpeed, dashSpdMulti, dashCharges;
    float curDashTime, dashChargeTime, curDashChargeTime, currentMana, perfectDodgeProgress;
    bool dashing;

    Vector2 direction;
    Animator animator;
    Movement movementScript;
    Health health;
    SpriteRenderer spriteRenderer;
    Inventory inventory;

    public Vector2 Movement { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        BeingType = BeingType.Friendly;
        health?.Initialize((int)resetStats[PlayerStat.Health], (int)resetStats[PlayerStat.Defense]);
    }

    private void Awake()
    {
        baseSpeed = 250;
        curSpeed = baseSpeed;
        dashSpdMulti = 3;
        curDashTime = 0;
        dashCharges = 3;
        dashChargeTime = 0.75f;
        perfectDodgeProgress = -1;
        curDashChargeTime = 0;
        dashing = false;

        inventory = Inventory.Instance;
        temporaryFlatBuffs = new ();
        temporaryPercentBuffs = new ();

        resetStats = new ()
        {
            { PlayerStat.Attack, 10 },
            { PlayerStat.Magic, 10 },
            { PlayerStat.Defense, 1 },
            { PlayerStat.Health, 100 },
            { PlayerStat.Mana, 100 },
            { PlayerStat.ManaRegen, 5 },
            { PlayerStat.ReactionAffinity, 10 },
            { PlayerStat.StaggerMultiplier, 1 },
            { PlayerStat.DefensePenetration, 0 },
            { PlayerStat.PhysicalDamageBonus, 0 },
            { PlayerStat.FireDamageBonus, 0 },
            { PlayerStat.WaterDamageBonus, 0 },
            { PlayerStat.WindDamageBonus, 0 },
            { PlayerStat.EarthDamageBonus, 0 },
            { PlayerStat.LightningDamageBonus, 0 },
            { PlayerStat.NatureDamageBonus, 0 },
            { PlayerStat.IceDamageBonus, 0 }
        };

        ResetStats();
        SetMana(CalculateStat(PlayerStat.Mana));

        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movementScript = GetComponent<Movement>();
        health = GetComponent<Health>();

        animator.SetFloat("MoveSpeed", curSpeed / 400f);
        
        EventManager.OnInventoryUpdated.AddListener(UpdateEquipmentStats);
        EventManager.OnEnemyDataAcquired.AddListener(DealDamage);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        if (direction.x != 0) spriteRenderer.flipX = direction.x > 0;

        HandleDash();
        HandleMana();
    }

    void FixedUpdate()
    {
        direction = Movement;
        movementScript.SetMovement(Movement.normalized * Time.deltaTime * curSpeed);
    }

    IEnumerator BuffCoroutine(StatBoost boost, BoostType boostType, float duration)
    {
        List<StatBoost> boostList = boostType == BoostType.Percentage ? temporaryPercentBuffs : temporaryFlatBuffs;
        boostList.Add(boost);
        yield return new WaitForSeconds(duration);
        boostList.Remove(boost);
    }

    float GetTemporaryBuff(PlayerStat playerStat, BoostType boostType)
    {
        List<StatBoost> buffList = boostType == BoostType.Percentage ? temporaryPercentBuffs : temporaryFlatBuffs;
        return buffList.Sum(buff => buff.Stat == playerStat ? buff.Amount : 0);
    }

    void ToggleDash()
    {
        health.ToggleInvincibility();
        dashing = !dashing;
        curSpeed = baseSpeed * (dashing ? dashSpdMulti : 1);

        if (dashing) return;
        perfectDodgeProgress = -1;
        Time.timeScale = 1;
    }

    void ResetStats()
    {
        percentageStats = GetEmptyStats();
        baseStats = GetEmptyStats();
        flatStats = GetEmptyStats();

        temporaryPercentBuffs.Clear();
        temporaryFlatBuffs.Clear();
    }

    float CalculateStat(PlayerStat stat)
    {
        if (stat == PlayerStat.None) return 0;
        if (Stats.IsPercentage(stat)) return resetStats[stat] + percentageStats[stat] + GetTemporaryBuff(stat, BoostType.Percentage);
        else return (resetStats[stat] + baseStats[stat]) * (1 + percentageStats[stat] + GetTemporaryBuff(stat, BoostType.Percentage)) + flatStats[stat] + GetTemporaryBuff(stat, BoostType.Flat);
    }

    void DealDamage(Collider2D[] colliders, AttackData attackData)
    {
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.Equals(gameObject)) continue;
            if (collider.GetComponent<Health>() is Health health)
            {
                if (health.IsInvincible) continue;
                int damage = Mathf.RoundToInt(attackData.Damage * damageConstant / (damageConstant + CalculateStat(PlayerStat.Attack)));
                health.TakeDamage(damage, attackData.Element, attackData.Origin);
                if (health.HP <= 0) EventManager.OnKill.Invoke();
            }
            if (collider.GetComponent<Stagger>() is Stagger stagger)
            {
                int staggerDamage = Mathf.RoundToInt(attackData.Stagger * CalculateStat(PlayerStat.StaggerMultiplier));
                stagger.TakeStagger(staggerDamage, attackData.Origin);
            }
            if (collider.GetComponent<Movement>() is Movement movement)
            {
                movement.ApplyKnockback(attackData.Origin, attackData.Knockback);
            }
            if (collider.GetComponent<DebuffManager>() is DebuffManager debuffManager)
            {
                DebuffData data = attackData.DebuffData;
                
                if (!data.Debuff) continue;
                debuffManager.InflictDebuff(data.Debuff, data.Duration, data.Strength);
            }
        }
    }

    void HandleDash()
    {
        curDashTime -= Time.deltaTime;

        if (perfectDodgeProgress >= 0 && perfectDodgeProgress <= 1)
        {
            Time.timeScale = 1 - slowCurve.Evaluate(perfectDodgeProgress) * perfectDodgeTimeScale;
            perfectDodgeProgress += Time.deltaTime / perfectDodgeDuration;
            
            if (perfectDodgeProgress > 1)
            {
                Time.timeScale = 1;
                perfectDodgeProgress = -1;
            }
        }

        if (dashing) return;
        animator.CrossFade("Player" + (direction.magnitude == 0 ? "Idle" : "Walk"), 0, 0);

        if (dashCharges >= 3) return;
        if (curDashChargeTime > 0)
        {
            curDashChargeTime -= Time.deltaTime;
        }
        else
        {
            dashCount.GetChild(dashCharges).gameObject.SetActive(true);
            dashCharges++;
            curDashChargeTime = dashChargeTime;
        }
    }

    void HandleMana()
    {
        if (currentMana >= CalculateStat(PlayerStat.Mana)) return;
        float manaRegenRate = 1 + CalculateStat(PlayerStat.ManaRegen) / 100;
        ReplenishMana(baseResourceRegen * manaRegenRate * Time.deltaTime);
    }

    Dictionary<PlayerStat, float> GetEmptyStats()
    {
        Dictionary<PlayerStat, float> output = new ();
        for (int i = 0; i < Enum.GetNames(typeof(PlayerStat)).Length; i++)
        {
            output.Add((PlayerStat)i, 0);
        }
        return output;
    }

    public string GetStats()
    {
        string output = "Stats: ";
        for (int i = 1; i < Enum.GetNames(typeof(PlayerStat)).Length; i++)
        {
            PlayerStat stat = (PlayerStat)i;
            string statStr = "";
            // in case your wondering this foreach loop adds spaces before each capital letter so that we can convert "directly" from the enum to a string lol
            foreach (char str in stat.ToString())
            {
                if (str.ToString().ToUpper().Equals(str)) statStr += " ";
                statStr += str;
            }
            output += $"<br>{statStr}: {StatBoost.GetValueAsString(stat, CalculateStat(stat))}";
        }
        return output;
    }

    public void IncrementMana(float amount)
    {
        SetMana(currentMana + amount);
    }

    public void SetMana(float amount)
    {
        float maxMana = CalculateStat(PlayerStat.Mana);
        currentMana = Mathf.Clamp(amount, 0, maxMana);
        onManaChanged?.Invoke(currentMana / maxMana);
    }

    public void OnDash()
    {
        if (dashing) return;
        if (dashCharges <= 0) return;
        dashCharges--;
        dashCount.GetChild(dashCharges).gameObject.SetActive(false);
        animator.CrossFade("PlayerDash" + (direction.x > 0 ? "Right" : "Left"), 0, 0);
    }

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }

    public void OnPerfectDodge()
    {
        if (!dashing) return;
        if (!health.IsInvincible) return;
        float manaRegenRate = 1 + CalculateStat(PlayerStat.ManaRegen) / 100;
        IncrementMana(perfectDodgeResourceGain * manaRegenRate);
        Time.timeScale = perfectDodgeTimeScale;
        perfectDodgeProgress = 0;
    }

    public void UpdateEquipmentStats()
    {
        ResetStats();
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < inventory.GetEquipmentCount(); i++)
        {
            Equipment equipInst = inventory.GetEquipmentAtIndex(i);

            if (equipInst == null) continue;
            ArmorSO armor = equipInst.EquipmentSO as ArmorSO;

            if (armor == null) continue;
            baseStats[PlayerStat.Defense] += armor.Defense;

            foreach (StatBoost substat in armor.Substats)
            {
                float amount = substat.Amount;
                PlayerStat stat = substat.Stat;
                BoostType boostType = Stats.IsPercentage(stat) ? BoostType.Percentage : BoostType.Flat;
                IncrementStat(stat, amount, boostType);
            }
        }
    }

    public void IncrementStat(PlayerStat stat, float amount, BoostType boostType)
    {
        switch (boostType)
        {
            case BoostType.Flat:
                flatStats[stat] += amount;
                break;
            case BoostType.Percentage:
                percentageStats[stat] += amount;
                break;
        }
    }

    public bool ConsumeMana(float amount)
    {
        if (amount > currentMana) return false;
        IncrementMana(-amount);
        return true;
    }

    public void ReplenishMana(float amount)
    {
        IncrementMana(amount);
    }

    public void AddTemporaryBuff(PlayerStat stat, float amount, float duration, BoostType boostType)
    {
        StatBoost buff = new (stat, amount);
        StartCoroutine(BuffCoroutine(buff, boostType, duration));
    }
}

public enum BoostType { Percentage, Flat }

public struct AttackData
{
    Element element;
    Vector2 origin;
    DebuffData debuffData;
    int damage, stagger;
    float knockback;

    public AttackData(Element element, Vector2 origin, int damage, int stagger, float knockback, DebuffData debuffData = new ())
    {
        this.element = element;
        this.origin = origin;
        this.debuffData = debuffData;
        this.damage = damage;
        this.stagger = stagger;
        this.knockback = knockback;
    }

    public Element Element { get => element; }
    public Vector2 Origin { get => origin; }
    public DebuffData DebuffData => debuffData;
    public int Damage { get => damage; }
    public int Stagger { get => stagger; }
    public float Knockback { get => knockback; }
}