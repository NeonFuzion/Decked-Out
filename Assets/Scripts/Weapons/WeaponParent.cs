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
    int curAnimIndex, currentHotbarIndex;

    Weapon weapon;
    Inventory inventory;
    Vector2 mousePosition;

    private void Awake()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateWeapon);
        EventManager.AddOnKillListener(InvokeOnKill);
    }

    // Start is called before the first frame update
    void Start()
    {
        attacking = false;
        curAnimIndex = 0;
        
        inventory = Inventory.Instance;
    }

    // Update is called once per frame
    void Update()
    {
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

    void UpdateWeapon()
    {
        Equipment[] weapons = inventory.Equiped.Where(equip => equip as Weapon).ToArray();
        
        if (weapons.Length == 0) return;
        weapon = weapons[currentHotbarIndex] as Weapon;

        if (weapon as Sword)
        {
            Sword sword = weapon as Sword;
            slash.parent.localPosition = Vector3.right * sword.AttackRange;
            slash.localScale = Vector3.one * sword.AttackRange;
        }
    }

    void InvokeOnKill()
    {
        onKill?.Invoke();
    }

    public void Attack()
    {
        if (attacking) return;

        attacking = true;
        weaponSpriteRenderer.sprite = weapon.Sprite;

        weapon.AttackAnimationHandle(curAnimIndex, transform);
        curAnimIndex = weapon.GetNextAnimationIndex(curAnimIndex);
    }

    public void OnAttackHit()
    {
        weapon.AttackActionHandle(curAnimIndex, slash, mousePosition);
    }

    public void OnAttackFinish()
    {
        attacking = false;
        slashAnimator.CrossFade("Idle", 0, 0);
    }

    public void OnWeaponIdle()
    {
        if (attacking) attacking = false;
    }

    public void SetHotbarIndex(int index)
    {
        currentHotbarIndex = index;
    }
}