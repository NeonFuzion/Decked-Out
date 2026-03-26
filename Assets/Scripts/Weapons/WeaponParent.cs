using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WeaponParent : MonoBehaviour
{
    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] Transform slash, damageOrigin;
    [SerializeField] Animator slashAnimator;
    [SerializeField] UnityEvent onAttack, onEnemyHit, onKill;

    bool attacking;
    int curAnimIndex;

    Weapon weapon;
    Shooter shooter;
    Inventory inventory;
    Animator animator;
    Vector2 mousePosition;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Instance;
        animator = transform.GetChild(1).GetComponent<Animator>();
        shooter = GetComponent<Shooter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!weapon) return;
        mousePosition = MainCamera.MouseWorldPosition();

        switch (weapon.WeaponHoldStyle)
        {
            case WeaponHoldStyle.Mouse:
                if (attacking) break;
                Vector2 direction = (Vector3)mousePosition - transform.position;
                //weaponSpriteRenderer.flipX = direction.x < 0;
                weaponSpriteRenderer.sortingOrder = -(int)Mathf.Sign(direction.y);
                transform.right = direction.normalized;
                break;
            case WeaponHoldStyle.Static:
                transform.right = Vector2.right * (mousePosition.x > 0 ? 1 : -1);
                break;
        }
    }

    public void Attack()
    {
        if (attacking) return;

        attacking = true;

        weapon.AttackAnimationHandle(curAnimIndex, transform, animator);
    }

    public void OnAttackHit()
    {
        weapon.AttackActionHandle(curAnimIndex, damageOrigin, mousePosition, shooter);
    }

    public void OnAttackFinish()
    {
        OnWeaponIdle();
        slashAnimator.CrossFade("Idle", 0, 0);
    }

    public void OnWeaponIdle()
    {
        if (!attacking) return;
        attacking = false;
        string lastAnimation = weapon.GetAnimationByIndex(curAnimIndex);
        curAnimIndex = weapon.GetNextAnimationIndex(curAnimIndex);

        if (!lastAnimation.Equals(weapon.GetAnimationByIndex(curAnimIndex))) return;
        animator.CrossFade("WeaponIdle", 0, 0);
    }

    public void UpdateWeapon(Weapon weapon)
    {
        this.weapon = weapon;

        animator?.CrossFade("WeaponIdle", 0, 0);
        attacking = false;
        curAnimIndex = 0;
        weaponSpriteRenderer.sprite = weapon.Sprite;

        if (weapon as Sword)
        {
            Sword sword = weapon as Sword;
            damageOrigin.localPosition = Vector3.right * sword.AttackRange;
            slash.parent.localPosition = Vector3.right * sword.AttackRange;
            slash.localScale = Vector3.one * sword.AttackRange;
        }
    }
}