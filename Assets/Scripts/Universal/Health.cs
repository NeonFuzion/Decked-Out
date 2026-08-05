using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float damageFlashDuration = 0.1f;
    [SerializeField] int hp, maxHp, def, defenseConstant = 100;
    [SerializeField] bool invincible;
    [SerializeField] Color damageFlashColor = Color.white;
    [SerializeField] GameObject prefabDmgObj, prefabHitEffect, prefabHealth;
    [SerializeField] Transform healthBarTarget;
    [SerializeField] HealthBar existingHealthBar;
    [SerializeField] UnityEvent onDeath, onHit;

    public int HP { get => hp; }
    public int MaxHP { get => maxHp; }
    public int Def { get => def; }
    public bool Invincible { get => invincible; }
    public UnityEvent OnDeath { get => onDeath; }

    HealthBar healthBar;
    Transform healthBarCanvas;
    List<SpriteRenderer> spriteRenderers;

    void Start()
    {
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();

        CreateHealthBar();
    }

    void Update () {
		float emission = Mathf.PingPong (Time.time, damageFlashDuration);
		Color baseColor = damageFlashColor; //Replace this with whatever you want for your base color at emission level '1'

		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);

        spriteRenderers.ForEach(spriteRenderer => {
		    Material mat = spriteRenderer.material;
            mat.SetColor ("_EmissionColor", finalColor);
        });
	}

    public void TakeDamage(int amount, Element element, Vector2 attackOrigin)
    {
        onHit.Invoke();
        if (invincible) return;
        int finalDamage = Mathf.RoundToInt(amount * defenseConstant / (defenseConstant + def));
        hp -= finalDamage;
        Instantiate(prefabHitEffect).GetComponent<HitEfect>().Initialize(transform.position);
        if (prefabDmgObj) SpawnDamageNumber(attackOrigin, element, finalDamage, false);

        if (amount < 0) return;
        if (healthBar) healthBar.SetFill((float)hp / maxHp);

        if (hp > 0) return;
        onDeath?.Invoke();

        if (!healthBarCanvas) return;
        Destroy(healthBarCanvas.gameObject);
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
        }
    }

    void SpawnDamageNumber(Vector2 incomingAttack, Element element, int amount, bool isHeal)
    {
        Vector2 direction = incomingAttack == new Vector2() ? (Vector2)transform.position : (incomingAttack - (Vector2)transform.position);
        GameObject dmgObj = Instantiate(prefabDmgObj, transform.position, Quaternion.identity);
        dmgObj.GetComponent<DamageObject>().Instantiate((isHeal ? -1 : 1) * amount, isHeal, direction, element);
    }
}
