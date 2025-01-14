using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
    float curLifetime;

    Projectile projectile;
    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        curLifetime += Time.deltaTime;
        if (curLifetime >= projectile.Lifespan) Destroy(gameObject);
        projectile.Movement(rigidbody);

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, 1))
        {
            if (col.gameObject == gameObject) return;
            if (col.gameObject.GetComponent<Player>()) return;
            col.gameObject.GetComponent<Health>()?.TakeDamage(projectile.Damage, transform.position);

            if (projectile.IsPiercing) return;
            Destroy(gameObject);
        }
    }
    
    public void Instantiate(Projectile projectile)
    {
        this.projectile = projectile;
        curLifetime = 0;
        GetComponent<SpriteRenderer>().sprite = projectile.Sprite;
        rigidbody = GetComponent<Rigidbody2D>();
    }
}
