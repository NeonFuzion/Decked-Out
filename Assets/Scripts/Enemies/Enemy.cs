using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Being
{
    [SerializeField] protected int atk, spd, detectDistance;
    [SerializeField] protected Animator animator;
    [SerializeField] protected LayerMask wallLayer;

    protected Rigidbody2D rb;
    protected Transform target;

    // Start is called before the first frame update
    protected void Start()
    {
        BeingType = BeingType.Hostile;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected Transform FindPlayer(Vector2 detectPoint, int radius)
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(detectPoint, radius))
        {
            Player script = col.GetComponent<Player>();

            if (!script) continue;
            return col.transform;
        }
        return null;
    }

    protected void SearchTarget(Vector2 detectPoint, int radius)
    {
        target = FindPlayer(detectPoint, radius);
    }

    protected void Movement()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * spd;
    }

    protected void Movement(Vector3 targetPosition)
    {
        if (!target) return;
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * spd;
    }

    protected void Telegraph()
    {

    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
