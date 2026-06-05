using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player : Being
{
    [SerializeField] float baseResourceRegen;
    [SerializeField] Transform sprite, dashCount;
    [SerializeField] UnityEvent<float> onManaChanged;
    [SerializeField] UnityEvent<int, bool> onDamageInflicted;

    Dictionary<PlayerStat, float> resetStats, baseStats, percentageStats, flatStats;

    int baseSpeed, curSpeed, dashSpdMulti, dashCharges;
    float curDashTime, dashChargeTime, curDashChargeTime, currentMana;
    bool dashing;

    Vector2 direction;
    Animator animator;
    Rigidbody2D rb;
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
        curDashChargeTime = 0;
        dashing = false;
        inventory = Inventory.Instance;

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
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        animator.SetFloat("MoveSpeed", curSpeed / 400f);
        
        EventManager.AddOnInventoryUpdatedListener(UpdateEquipmentStats);
        EventManager.AddOnEnemyDataAcquiredListener(DealDamage);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        spriteRenderer.flipX = direction.x > 0;

        HandleDash();
        HandleMana();
    }

    void FixedUpdate()
    {
        direction = Movement;
        rb.linearVelocity = Movement.normalized * Time.deltaTime * curSpeed;
    }

    void ToggleDash()
    {
        health?.ToggleInvincibility();
        dashing = !dashing;
        curSpeed = baseSpeed * (dashing ? dashSpdMulti : 1);
    }

    void ResetStats()
    {
        percentageStats = GetEmptyStats();
        baseStats = GetEmptyStats();
        flatStats = GetEmptyStats();
    }

    float CalculateStat(PlayerStat stat)
    {
        if (stat == PlayerStat.None) return 0;
        float output = 0;
        if (Stats.IsPercentage(stat))
            output = resetStats[stat] + percentageStats[stat];
        else
            output = (resetStats[stat] + baseStats[stat]) * (1 + percentageStats[stat]) + flatStats[stat];
        return output;
    }

    void DealDamage(Collider2D[] colliders, AttackData attackData)
    {
        colliders.ToList().ForEach(collider =>
        {
            if (collider.gameObject.Equals(gameObject)) return;
            Health health = collider.GetComponent<Health>();
            Stagger stagger = collider.GetComponent<Stagger>();

            if (!health) return;
            int damage = Mathf.RoundToInt(attackData.Damage * (1 + Mathf.Log(CalculateStat(PlayerStat.Attack)) / 20));
            int staggerDamage = Mathf.RoundToInt(attackData.Stagger * CalculateStat(PlayerStat.StaggerMultiplier));
            health.TakeDamage(damage, attackData.Element, attackData.Origin);
            stagger?.TakeStagger(staggerDamage);

            if (health.HP > 0) return;
            EventManager.InvokeOnKill();
        });
    }
    void HandleDash()
    {
        curDashTime -= Time.deltaTime;

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

    public void UpdateEquipmentStats()
    {
        ResetStats();
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < inventory.GetEquipmentCount(); i++)
        {
            EquipmentInstance equipInst = inventory.GetEquipment(i);

            if (equipInst == null) continue;
            ArmorSO armor = equipInst.EquipmentData as ArmorSO;

            if (armor == null) continue;
            baseStats[PlayerStat.Defense] += armor.Defense;

            armor.Substats.ToList().ForEach(substat =>
            {
                float amount = substat.Amount;
                PlayerStat stat = substat.Stat;
                BoostType boostType = Stats.IsPercentage(stat) ? BoostType.Percentage : BoostType.Flat;
                IncrementStat(stat, amount, boostType);
            });
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
}

public enum BoostType { Percentage, Flat }

public class AttackData
{
    Element element;
    Vector2 origin;
    int damage, stagger, knockback;

    public AttackData(Element element, Vector2 origin, int damage, int stagger, int knockback = 1)
    {
        this.element = element;
        this.origin = origin;
        this.damage = damage;
        this.stagger = stagger;
        this.knockback = knockback;
    }

    public Element Element { get => element; }
    public Vector2 Origin { get => origin; }
    public int Damage { get => damage; }
    public int Stagger { get => stagger; }
    public int Knockback { get => knockback; }
}

public abstract class AttackAugment
{
    public void Initialize() {}

    public abstract void AugmentAttack(Collider2D[] colliders, Dictionary<PlayerStat, float> percentageBoosts, Dictionary<PlayerStat, float> flatBoosts);
}