using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int hp, maxHp, def;
    [SerializeField] float knockbackResistance = 0;
    [SerializeField] bool invincible;
    [SerializeField] UnityEvent onDeath, onHit;
    [SerializeField] GameObject prefabDmgObj, prefabHitEffect, prefabHealth;
    [SerializeField] Transform healthBarTarget;
    [SerializeField] HealthBar existingHealthBar;

    public int HP { get => hp; }
    public int MaxHP { get => maxHp; }
    public int Def { get => def; }
    public bool Invincible { get => invincible; }
    public UnityEvent OnDeath { get => onDeath; }

    HealthBar healthBar;
    StaggerBar staggerBar;
    Transform healthBarCanvas;

    void Start() { CreateHealthBar(); }

    public void TakeDamage(int amount, Element element, Vector2 incomingAttack = new Vector2(), float knockback = 1)
    {
        onHit.Invoke();
        if (invincible) return;
        int finalDamage = Mathf.RoundToInt((1 - 0.01f * Mathf.Log(def + 1, 1.2f)) * amount);
        hp -= finalDamage;

        Instantiate(prefabHitEffect).GetComponent<HitEfect>().Initialize(transform.position);

        if (prefabDmgObj) SpawnDamageNumber(incomingAttack, element, finalDamage, false);
        if (amount < 0) return;

        Enemy enemy = GetComponent<Enemy>();
        if (incomingAttack != Vector2.zero && enemy)
        {
            StopAllCoroutines();
            StartCoroutine(ApplyHitEffects(incomingAttack, knockback, enemy));
        }

        if (hp > 0)
        {
            if (healthBar) healthBar.SetFill((float)hp / maxHp);
            return;
        }

        onDeath?.Invoke();
        if (healthBarCanvas) Destroy(healthBarCanvas.gameObject);
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxHp) hp = maxHp;

        if (healthBar) healthBar.SetFill((float)hp / maxHp);
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

        CreateHealthBar();
    }

    public void CreateHealthBar()
    {
        if (existingHealthBar)
        {
            healthBar = existingHealthBar;
        }
        else if (prefabHealth)
        {
            healthBarCanvas = Instantiate(prefabHealth, healthBarTarget.position, Quaternion.identity).transform;
            healthBar = healthBarCanvas.GetComponentInChildren<HealthBar>();
            healthBar.Initialize((float)hp / maxHp);
            staggerBar = healthBarCanvas.GetComponentInChildren<StaggerBar>();
            staggerBar?.Initialize(1f);
        }
    }

    void SpawnDamageNumber(Vector2 incomingAttack, Element element, int amount, bool isHeal)
    {
        Vector2 direction = incomingAttack == new Vector2() ? (Vector2)transform.position : (incomingAttack - (Vector2)transform.position);
        GameObject dmgObj = Instantiate(prefabDmgObj, transform.position, Quaternion.identity);
        dmgObj.GetComponent<DamageObject>().Instantiate((isHeal ? -1 : 1) * amount, isHeal, direction, element);
    }

    IEnumerator ApplyHitEffects(Vector2 incomingAttack, float knockback, Enemy enemy)
    {
        bool wasEnabled = enemy.enabled;
        enemy.enabled = false;
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        Vector2 oldVelocity = rigidbody.linearVelocity;
        rigidbody.AddForce(((Vector2)transform.position - incomingAttack) * knockback * (1 - knockbackResistance), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        enemy.enabled = wasEnabled;
        rigidbody.linearVelocity = oldVelocity;
    }
}
