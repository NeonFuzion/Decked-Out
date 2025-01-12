using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponParent : MonoBehaviour
{
    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] Transform slash;
    [SerializeField] UnityEvent<int, float> onAttack;
    [SerializeField] UnityEvent onEnemyHit, onKill;

    bool swungSide, attacking;
    int curAnimIndex;

    Animator animr, slashAnimr;
    Vector2 mousePos;
    Weapon weapon;

    private void Awake()
    {
        EventManager.AddOnEquipmentUpdatedListener(UpdateWeapon);
    }

    // Start is called before the first frame update
    void Start()
    {
        swungSide = false;
        attacking = false;

        animr = transform.GetChild(0).GetComponent<Animator>();
        slashAnimr = transform.GetChild(1).GetComponent<Animator>();
        curAnimIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnEnemyHit()
    {
        onEnemyHit.Invoke();
    }

    public void SwordAnimHandle()
    {
        Sword sword = weapon as Sword;
        slash.position = transform.position + transform.right * sword.AttackRange;
        slash.localScale = Vector3.one * sword.AttackRange * 1.2f;

        slashAnimr.SetFloat("AtkSpdMulti", sword.AttackSpeed);
        slashAnimr.CrossFade(sword.GetSlashAnimationByIndex(curAnimIndex), 0, 0);
    }

    public void MagicWeaponAnimHandle()
    {
        MagicWeapon mageWeapon = weapon as MagicWeapon;
        GameObject projectile = Instantiate(mageWeapon.PrefabProjectile,  transform.GetChild(0).GetChild(0).position, Quaternion.identity);
        projectile.GetComponent<ProjectileObject>().Instantiate(mageWeapon.Projectile);
    }

    public void UpdateWeapon(Equipment[] equiped)
    {
        if (!equiped[0]) return;
        weapon = equiped[0] as Weapon;
    }
    
    public void Attack()
    {
        if (attacking) return;

        attacking = true;
        weaponSpriteRenderer.sprite = weapon.Sprite;
        transform.right = ((Vector3)mousePos - transform.position).normalized;

        onAttack?.Invoke(curAnimIndex, weapon.AttackSpeed);

        animr.SetFloat("AttackSwingSpeed", weapon.AttackSpeed);
        animr.CrossFade(weapon.GetAnimationByIndex(curAnimIndex), 0, 0);

        if (weapon as Sword) SwordAnimHandle();
        else if (weapon as MagicWeapon) MagicWeaponAnimHandle();

        curAnimIndex = weapon.GetNextAnimationIndex(curAnimIndex);
    }

    public void OnAttackFinish()
    {
        attacking = false;
        animr.CrossFade("WeaponIdle", 0, 0);
    }

    public void DetectEntities(int damage, bool isCrit)
    {
        Sword sword = weapon as Sword;
        Vector3 hitPos = transform.right * sword.AttackRange * 0.5f;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position + hitPos, sword.AttackRange))
        {
            if (col.GetComponent<Player>()) continue;
            Health health = col.GetComponent<Health>();

            if (!health) continue;
            health.TakeDamage(damage, transform.parent.position, isCrit, sword.Knockback);

            if (health.HP > 0) continue;
            onKill?.Invoke();
        }
    }

    public void AddOnAttackListener(UnityAction<int, float> unityAction)
    {
        onAttack?.AddListener(unityAction);
    }
}