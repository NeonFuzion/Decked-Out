using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fountain : TerrainObject
{
    [SerializeField] Sprite activeSprite, inactiveSprite;

    bool isActive;

    Animator animator;
    SpriteRenderer spriteRenderer;
    UnityEvent<GameObject> onUnactivate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if (!isActive) return;
        Collider2D player = Physics2D.OverlapCircle(transform.position, 5, LayerMask.GetMask("Player"));

        if (!player) return;
        player.GetComponent<Health>().Heal(Random.Range(10, 30));
        animator.CrossFade("Empty", 0, 0);
    }

    public override void Initialize(bool isActive, UnityAction<GameObject> unityAction)
    {
        this.isActive = isActive;
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        onUnactivate = new ();
        onUnactivate.AddListener(unityAction);

        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
    }
}
