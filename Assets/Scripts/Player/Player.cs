using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] Transform head, dashCount;
    [SerializeField] UnityEvent<int, bool> onDamageInflicted;

    Dictionary<PlayerStat, int> baseStats, stats;

    int baseSpeed, curSpeed, dashSpdMulti, dashCharges;
    float curDashTime, dashChargeTime, curDashChargeTime;
    bool dashing;

    Vector2 direction;
    Animator animr;
    Rigidbody2D rb;

    public Vector2 Movement { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Health>()?.Initialize(baseStats[PlayerStat.Health], baseStats[PlayerStat.Defense]);
        EventManager.AddOnEquipmentUpdatedListener(UpdateStats);
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

        stats = SetToBaseStats();
        baseStats = SetToBaseStats();

        animr = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        curDashTime -= Time.deltaTime;
        head.localScale = new Vector3(direction.x != 0 ? (direction.x < 0 ? 1 : -1) : head.localScale.x, 1, 1);

        if (dashing) return;
        animr.CrossFade("Player" + (direction.magnitude == 0 ? "Idle" : "Walk"), 0, 0);

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

    private void FixedUpdate()
    {
        direction = Movement;
        rb.linearVelocity = Movement.normalized * Time.deltaTime * curSpeed;
    }

    void DashHandle()
    {
        GetComponent<Health>()?.SetInvincibility();
        dashing = !dashing;
        curSpeed = baseSpeed * (dashing ? dashSpdMulti : 1);
    }

    Dictionary<PlayerStat, int> SetToBaseStats()
    {
        return new Dictionary<PlayerStat, int>()
        {
            { PlayerStat.Attack, 10 },
            { PlayerStat.Defense, 1 },
            { PlayerStat.Health, 100 },
            { PlayerStat.CriticalChance, 5 },
            { PlayerStat.CriticalDamage, 50 },
            { PlayerStat.EnergyRecharge, 100 }
        };
    }

    public string GetStats()
    {
        return "Stats" +
            $"\nHealth: {stats[PlayerStat.Health]}" +
            $"\nAttack: {stats[PlayerStat.Attack]}" +
            $"\nDefense: {stats[PlayerStat.Defense]}" +
            $"\nCritical Chance: {stats[PlayerStat.CriticalChance]}%" +
            $"\nCritical Damage: {stats[PlayerStat.CriticalDamage]}%";
    }

    public void OnDash()
    {
        if (dashing) return;
        if (dashCharges <= 0) return;
        dashCharges--;
        dashCount.GetChild(dashCharges).gameObject.SetActive(false);
        animr.CrossFade("PlayerDash" + (direction.x > 0 ? "Right" : "Left"), 0, 0);
    }

    public void UpdateStats(Equipment[] equiped)
    {
        stats = SetToBaseStats();

        Dictionary<PlayerStat, float> percentageStatBoosts = new Dictionary<PlayerStat, float>();
        foreach (Equipment equipment in equiped)
        {
            if (!equipment) continue;
            if (equipment as Weapon)
            {
                stats[PlayerStat.Attack] = baseStats[PlayerStat.Attack] + (int)(equipment as Weapon).Attack;
            }
            if (equipment as Accessory)
            {
                Accessory accessory = equipment as Accessory;
                if (percentageStatBoosts.ContainsKey(accessory.MainStat.Stat))
                    percentageStatBoosts[accessory.MainStat.Stat] += accessory.MainStat.Amount;
                else percentageStatBoosts.Add(accessory.MainStat.Stat, accessory.MainStat.Amount);
            }
        }

        foreach (PlayerStat pStat in percentageStatBoosts.Keys)
        {
            if (!Stats.IsPercentage(pStat))
                stats[pStat] = Mathf.RoundToInt(stats[pStat] * (1 + percentageStatBoosts[pStat] / 100));
            else
                stats[pStat] += Mathf.RoundToInt(percentageStatBoosts[pStat]);
        }
    }

    public int GetStat(PlayerStat stat)
    {
        return stats[stat];
    }

    public void IncrementStat(PlayerStat stat, int amount)
    {
        stats[stat] += amount;
    }

    public void CalculateDamage()
    {
        int damage = stats[PlayerStat.Attack];
        float critMulti = 1;
        int chance = Random.Range(0, 100);
        if (chance <= stats[PlayerStat.CriticalChance]) critMulti += stats[PlayerStat.CriticalDamage] / 100f;
        damage = Mathf.RoundToInt(damage * critMulti);
        onDamageInflicted.Invoke(damage, critMulti > 1);
    }
}
