using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Stagger : MonoBehaviour
{
    [SerializeField] int maxStaggerPoints = 100;
    [SerializeField] int staggerDamage = 10;
    [SerializeField] float stunDuration = 2f;
    [SerializeField] Animator animator;
    [SerializeField] StaggerBar staggerBar;

    int currentStaggerPoints;
    bool isStaggered;

    Health health;

    void Start()
    {
        isStaggered = false;
        currentStaggerPoints = maxStaggerPoints;
        
        health = GetComponent<Health>();
    }

    public void TakeStagger(int amount)
    {
        if (isStaggered || amount <= 0) return;

        currentStaggerPoints -= amount;

        if (currentStaggerPoints <= 0)
        {
            currentStaggerPoints = 0;
            StartCoroutine(ApplyStagger());
        }

        if (staggerBar) staggerBar.SetFill((float)currentStaggerPoints / maxStaggerPoints);
    }

    IEnumerator ApplyStagger()
    {
        isStaggered = true;

        Enemy enemy = GetComponent<Enemy>();
        enemy.OnStagger();

        if (health) health.TakeDamage(staggerDamage, Element.Physical);

        if (staggerBar) staggerBar.SetRefilling(true);

        animator.CrossFade("StaggerEffect", 0, 0);
        float elapsed = 0f;
        while (elapsed < stunDuration)
        {
            elapsed += Time.deltaTime;
            currentStaggerPoints = Mathf.RoundToInt(Mathf.Lerp(0, maxStaggerPoints, elapsed / stunDuration));
            if (staggerBar) staggerBar.SetFill((float)currentStaggerPoints / maxStaggerPoints);
            yield return null;
        }

        currentStaggerPoints = maxStaggerPoints;
        if (staggerBar)
        {
            staggerBar.SetFill(1f);
            staggerBar.SetRefilling(false);
        }
        isStaggered = false;
        enemy.OnStaggerEnd();

        animator.CrossFade("HideEffect", 0, 0);
    }
}
