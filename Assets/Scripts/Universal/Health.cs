using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float healingMultiplier = 1;
    [SerializeField] int hp, def, defenseConstant = 50;
    [SerializeField] bool invincible;
    [SerializeField] GameObject prefabDmgObj, prefabHitEffect;
    [SerializeField] UnityEvent onDeath, onHit;
    [SerializeField] UnityEvent<float> onHealthChanged;

    public int HP { get => hp; }
    public int MaxHP { get => maxHp; }
    public int Defense { get => def; }
    public float HealingMultiplier => healingMultiplier;
    public bool IsInvincible { get => invincible; }
    public UnityEvent OnDeath { get => onDeath; }
    public UnityEvent<float> OnHealthChanged => onHealthChanged;

    int maxHp;

    Transform healthBarCanvas;

    void Start()
    {
        maxHp = hp;
    }

    void Update()
    {
        
    }

    public void TakeDamage(int amount, Element element, Vector2 attackOrigin)
    {
        onHit.Invoke();
        if (invincible) return;
        int finalDamage = Mathf.RoundToInt(amount * defenseConstant / (defenseConstant + def));
        hp -= finalDamage;
        onHealthChanged?.Invoke((float)hp / maxHp);
        Instantiate(prefabHitEffect).GetComponent<HitEfect>().Initialize(transform.position);
        if (prefabDmgObj) SpawnDamageNumber(attackOrigin, element, finalDamage, false);

        if (amount < 0) return;
        if (hp > 0) return;
        onDeath?.Invoke();

        if (!healthBarCanvas) return;
        Destroy(healthBarCanvas.gameObject);
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + Mathf.RoundToInt(amount * healingMultiplier), maxHp);
        if (prefabDmgObj) SpawnDamageNumber(Vector2.down, Element.Physical, amount, true);
    }

    public void SetInvincibility(bool invincible)
    {
        this.invincible = invincible;
    }

    public void ToggleInvincibility()
    {
        invincible = !invincible;
    }

    public void Initialize(int maxHp, int def)
    {
        this.def = def;
        this.maxHp = maxHp;
        hp = maxHp;
    }

    void SpawnDamageNumber(Vector2 incomingAttack, Element element, int amount, bool isHeal)
    {
        Vector2 direction = incomingAttack == new Vector2() ? (Vector2)transform.position : (incomingAttack - (Vector2)transform.position);
        GameObject dmgObj = Instantiate(prefabDmgObj, transform.position, Quaternion.identity);
        dmgObj.GetComponent<DamageObject>().Instantiate((isHeal ? -1 : 1) * amount, isHeal, direction, element);
    }

    public void SetDefense(int def)
    {
        this.def = def;
    }

    public void SetHealingMultiplier(float healingMultiplier)
    {
        this.healingMultiplier = healingMultiplier;
    }
}
