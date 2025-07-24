using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int atk, hp, def, spd, detectDistance;
    [SerializeField] protected Animator animator;

    protected Rigidbody2D rb;
    protected Transform target;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void SearchTarget(Vector2 detectPoint, int radius)
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(detectPoint, radius))
        {
            Player script = col.GetComponent<Player>();
            if (!script) continue;
            target = col.transform;
        }
    }

    protected void Movement()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;
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
