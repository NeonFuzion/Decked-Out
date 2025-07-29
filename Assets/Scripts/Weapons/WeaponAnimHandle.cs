using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAnimHandle : MonoBehaviour
{
    [SerializeField] UnityEvent onDamageInflicted, onAttackFinished, onWeaponIdled;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnAttackFinish()
    {
        onAttackFinished?.Invoke();
    }

    void DealDamage()
    {
        onDamageInflicted?.Invoke();
    }

    void IdleWeapon()
    {
        onWeaponIdled?.Invoke();
    }
}
