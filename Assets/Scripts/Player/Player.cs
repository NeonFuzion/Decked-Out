using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player : Being
{
    [SerializeField] Transform sprite, dashCount;
    [SerializeField] UnityEvent<int, bool> onDamageInflicted;

    Dictionary<PlayerStat, float> resetStats, baseStats, percentageStats, flatStats, stats;

    int baseSpeed, curSpeed, dashSpdMulti, dashCharges;
    float curDashTime, dashChargeTime, curDashChargeTime;
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

        ResetStats();
        resetStats = new ()
        {
            { PlayerStat.Attack, 10 },
            { PlayerStat.Magic, 10 },
            { PlayerStat.Defense, 1 },
            { PlayerStat.Health, 100 },
            { PlayerStat.ReactionAffinity, 10 },
            { PlayerStat.Mana, 50 },
            { PlayerStat.StaggerMultiplier, 100 },
            { PlayerStat.DefensePenetration, 0 },
            { PlayerStat.ManaRegeneration, 100 },
            { PlayerStat.PhysicalDamageBonus, 0 },
            { PlayerStat.FireDamageBonus, 0 },
            { PlayerStat.WaterDamageBonus, 0 },
            { PlayerStat.WindDamageBonus, 0 },
            { PlayerStat.EarthDamageBonus, 0 },
            { PlayerStat.LightningDamageBonus, 0 },
            { PlayerStat.NatureDamageBonus, 0 },
            { PlayerStat.IceDamageBonus, 0 }
        };

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
        curDashTime -= Time.deltaTime;
        spriteRenderer.flipX = direction.x > 0;

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

    void FixedUpdate()
    {
        direction = Movement;
        rb.linearVelocity = Movement.normalized * Time.deltaTime * curSpeed;
    }

    void DashHandle()
    {
        health?.SetInvincibility();
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

            if (!health) return;
            float damage = attackData.Multipliers.ToList().Sum(multiplier =>
            {
                try { return CalculateStat(multiplier.Stat) * multiplier.Amount; }
                catch (Exception) { return 0; }
            });
            health.TakeDamage(Mathf.RoundToInt(damage), attackData.Element, attackData.Origin);

            if (health.HP > 0) return;
            EventManager.InvokeOnKill();
        });
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
        for (int i = 0; i < Enum.GetNames(typeof(PlayerStat)).Length; i++)
        {
            PlayerStat stat = (PlayerStat)i;
            string statStr = "";
            foreach (char str in stat.ToString())
            {
                if (str.ToString().ToUpper().Equals(str)) statStr += " ";
                statStr += str;
            }
            output += $"/n{statStr}: {CalculateStat(stat)}";
        }
        return output;
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
            Equipment equipment = inventory.GetEquipment(i);

            if (!equipment) return;
            if (equipment as Weapon)
            {
                Weapon weapon = equipment as Weapon;
                baseStats[PlayerStat.Attack] = weapon.Attack;

                if (weapon.Substat.Stat == PlayerStat.None) return;
                percentageStats[weapon.Substat.Stat] = weapon.Substat.Amount;
            }
            else if (equipment as Armor)
            {
                Armor armor = equipment as Armor;
                baseStats[PlayerStat.Defense] += armor.Defense;

                if (armor.Substats.Length == 0) return;
                percentageStats[armor.SecondaryStat.Stat] += armor.SecondaryStat.Amount;
            }
        }
    }

    public void UpdateStats()
    {
        resetStats.Keys.ToList().ForEach(stat =>
        {
            if (Stats.IsPercentage(stat))
                stats[stat] = resetStats[stat] + percentageStats[stat];
            else
                stats[stat] = (resetStats[stat] + baseStats[stat]) * (1 + percentageStats[stat]) + flatStats[stat];
        });
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
}

public enum BoostType { Percentage, Flat }

public class AttackData
{
    Element element;
    Vector2 origin;
    StatBoost[] multipliers;

    public AttackData(Element element, Vector2 origin, StatBoost[] multipliers)
    {
        this.element = element;
        this.origin = origin;
        this.multipliers = multipliers;
    }

    public Element Element { get => element; }
    public Vector2 Origin { get => origin; }
    public StatBoost[] Multipliers { get => multipliers; }
}

public abstract class AttackAugment
{
    public void Initialize() {}

    public abstract void AugmentAttack(Collider2D[] colliders, Dictionary<PlayerStat, float> percentageBoosts, Dictionary<PlayerStat, float> flatBoosts);
}