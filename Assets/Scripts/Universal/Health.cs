using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] int hp, maxHp, def;
    [SerializeField] float knockbackResistance = 0;
    [SerializeField] bool invincible;
    [SerializeField] UnityEvent onDeath, onHit;
    [SerializeField] GameObject prefabDmgObj, prefabHitEffect, prefabHealth;
    [SerializeField] GameObject healthBarTarget, existingHealthBar;

    public int HP { get => hp; }
    public int MaxHP { get => maxHp; }
    public int Def { get => def; }
    public bool Invincible { get => invincible; }
    public UnityEvent OnDeath { get => onDeath; }

    Slider slider;
    GameObject healthBarCanvas, healthBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!slider) return;
        if (healthBarCanvas) healthBarCanvas.transform.position = healthBarTarget.transform.position;

        if (slider.value == hp) return;
        slider.value += (hp - slider.value) / 10;

        Image image = slider.GetComponentInChildren<Image>();
        if (hp < maxHp) image.color = Color.green;
        if (hp < maxHp * 0.6f) image.color = Color.yellow;
        if (hp < maxHp * 0.2f) image.color = Color.red;
    }

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

        if (hp > 0) return;
        onDeath?.Invoke();
        Destroy(healthBarCanvas);
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxHp) hp = maxHp;

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

    void SpawnDamageNumber(Vector2 incomingAttack, Element element, int amount, bool isHeal)
    {
        Vector2 direction = incomingAttack == new Vector2() ? (Vector2)transform.position : (incomingAttack - (Vector2)transform.position);
        GameObject dmgObj = Instantiate(prefabDmgObj, transform.position, Quaternion.identity);
        dmgObj.GetComponent<DamageObject>().Instantiate((isHeal ? -1 : 1) * amount, isHeal, direction, element);
    }

    IEnumerator ApplyHitEffects(Vector2 incomingAttack, float knockback, Enemy enemy)
    {
        enemy.enabled = false;
        GetComponent<Rigidbody2D>().AddForce(((Vector2)transform.position - incomingAttack) * knockback * (1 - knockbackResistance), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        enemy.enabled = true;
    }

    public void Initialize(int maxHp, int def)
    {
        this.def = def;
        this.maxHp = maxHp;
        hp = maxHp;

        if (existingHealthBar)
        {
            healthBar = existingHealthBar;
        }
        else
        {
            healthBarCanvas = Instantiate(prefabHealth, healthBarTarget.transform.position, Quaternion.identity);
            healthBar = healthBarCanvas.transform.GetChild(0).gameObject;
        }
        slider = healthBar.GetComponent<Slider>();
        slider.GetComponentInChildren<Image>().color = Color.green;
        slider.maxValue = maxHp;
        slider.value = hp;
    }
}
