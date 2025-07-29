using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponParent : MonoBehaviour
{
    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] Transform slash, weaponTip;
    [SerializeField] UnityEvent<int, float> onAttack;
    [SerializeField] UnityEvent onEnemyHit, onKill;

    bool attacking;
    int curAnimIndex;

    Animator animr;
    Weapon weapon;

    private void Awake()
    {
        EventManager.AddOnEquipmentUpdatedListener(UpdateWeapon);
        EventManager.AddOnKillListener(InvokeOnKill);
    }

    // Start is called before the first frame update
    void Start()
    {
        attacking = false;

        animr = transform.GetChild(0).GetComponent<Animator>();
        curAnimIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void OnEnemyHit()
    {
        onEnemyHit.Invoke();
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

        Vector2 mousePos = MainCamera.MousePosition;

        switch (weapon.WeaponHoldStyle)
        {
            case WeaponHoldStyle.Mouse:
                transform.right = ((Vector3)mousePos - transform.position).normalized;
                break;
            case WeaponHoldStyle.Static:
                transform.right = Vector2.right * (mousePos.x > 0 ? 1 : -1);
                break;
        }
        
        weapon.AttackAnimationHandle(curAnimIndex, transform);
        curAnimIndex = weapon.GetNextAnimationIndex(curAnimIndex);
    }

    public void OnAttackFinish()
    {
        attacking = false;
        animr.CrossFade("WeaponIdle", 0, 0);
    }

    public void OnWeaponIdle()
    {
        if (attacking) attacking = false;
    }

    public void DealDamage(int damage, bool isCrit)
    {
        weapon.AttackActionHandle(damage, isCrit, transform);
    }

    public void AddOnAttackListener(UnityAction<int, float> unityAction)
    {
        onAttack?.AddListener(unityAction);
    }

    public void InvokeOnKill() => onKill?.Invoke();
}