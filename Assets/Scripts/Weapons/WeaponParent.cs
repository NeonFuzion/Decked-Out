using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LightTransport;

public class WeaponParent : MonoBehaviour
{
    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] Transform slash, weaponTip, shooterPoint;
    [SerializeField] UnityEvent<int, float> onAttack;
    [SerializeField] UnityEvent onEnemyHit, onKill;

    bool swungSide, attacking;
    int curAnimIndex;

    Animator animr, slashAnimr;
    Vector2 mousePos;
    Weapon weapon;
    Shooter shooter;

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
        shooter = shooterPoint.GetComponent<Shooter>();
        curAnimIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void DetectEnemies(int damage, bool isCrit)
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

    void LaunchProjectiles(int damage, bool isCrit)
    {
        MagicWeapon magicWeapon = weapon as MagicWeapon;
        shooterPoint.position = weaponTip.position;
    
        int range = magicWeapon.ProjectileSpread;
        Vector2 offset = Vector2.zero;
        
        for (int i = 0; i < magicWeapon.ProjectileCount; i++)
        {
            if (magicWeapon.ProjectileCount > 1)
                offset = new Vector2(Random.Range(-range, range), Random.Range(-range, range));
            shooter.FireProjectile(magicWeapon.ProjectileData, ProjectileTargetType.Friendly, mousePos + offset, damage, isCrit, InvokeOnKill);
        }
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

        onAttack?.Invoke(curAnimIndex, weapon.AttackSpeed);

        animr.SetFloat("AttackSwingSpeed", weapon.AttackSpeed);
        animr.CrossFade(weapon.GetAnimationByIndex(curAnimIndex), 0, 0);

        switch (weapon.WeaponHoldStyle)
        {
            case WeaponHoldStyle.Mouse:
                transform.right = ((Vector3)mousePos - transform.position).normalized;
                break;
            case WeaponHoldStyle.Static:
                transform.right = Vector2.right * (mousePos.x > 0 ? 1 : -1);
                break;
        }
        
        if (weapon as Sword) SwordAnimHandle();

        curAnimIndex = weapon.GetNextAnimationIndex(curAnimIndex);
    }

    public void OnAttackFinish()
    {
        attacking = false;
        animr.CrossFade("WeaponIdle", 0, 0);
    }

    public void DealDamage(int damage, bool isCrit)
    {
        if (weapon as Sword) DetectEnemies(damage, isCrit);
        else if (weapon as MagicWeapon) LaunchProjectiles(damage, isCrit);
    }

    public void AddOnAttackListener(UnityAction<int, float> unityAction)
    {
        onAttack?.AddListener(unityAction);
    }

    public void InvokeOnKill() => onKill?.Invoke();
}