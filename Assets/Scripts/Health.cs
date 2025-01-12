using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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

    public void TakeDamage(int amount, Vector2 incomingAttack = new Vector2(), bool isCrit = false, float knockback = 1)
    {
        onHit.Invoke();
        if (invincible) return;
        int finalDamage = Mathf.RoundToInt((1 - 0.01f * Mathf.Log(def + 1, 1.2f)) * amount);
        hp -= finalDamage;

        Instantiate(prefabHitEffect).GetComponent<HitEfect>().Initialize(transform.position);

        if (prefabDmgObj) UpdateHealthBar(incomingAttack, finalDamage, isCrit);
        if (amount < 0) return;
        if (incomingAttack != Vector2.zero)
        {
            StopAllCoroutines();
            StartCoroutine(ApplyKnockback(incomingAttack, knockback));
        }

        if (hp > 0) return;
        onDeath?.Invoke();
        Destroy(gameObject);
        Destroy(healthBarCanvas);
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxHp) hp = maxHp;

        if (prefabDmgObj) UpdateHealthBar(Vector2.down, -amount, false);
    }

    public void SetInvincibility()
    {
        invincible = !invincible;
    }

    void UpdateHealthBar(Vector2 incomingAttack, int amount, bool isCrit)
    {
        Vector2 direction = incomingAttack == new Vector2() ? (Vector2)transform.position : (incomingAttack - (Vector2)transform.position);
        GameObject dmgObj = Instantiate(prefabDmgObj, transform.position, Quaternion.identity);
        dmgObj.GetComponent<DamageObject>().Instantiate(amount, direction);
        if (isCrit) dmgObj.GetComponent<TextMeshPro>().color = Color.red;
        if (amount < 0) dmgObj.GetComponent<TextMeshPro>().color = Color.green;
    }

    IEnumerator ApplyKnockback(Vector2 incomingAttack, float knockback)
    {
        Enemy enemy = GetComponent<Enemy>();
        if (!enemy) yield return null;
        enemy.enabled = false;
        GetComponent<Rigidbody2D>().AddForce(((Vector2)transform.position - incomingAttack) * knockback * (1 - knockbackResistance), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        enemy.enabled = true;
    }

    public void Initialize(int maxHP, int def)
    {
        this.def = def;
        this.maxHp = maxHP;
        hp = maxHP;

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
        slider.maxValue = maxHP;
        slider.value = hp;
    }
}
