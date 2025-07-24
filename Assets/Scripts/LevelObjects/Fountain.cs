using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fountain : RoomObject
{
    [SerializeField] Sprite activeSprite, inactiveSprite;

    Animator animator;
    SpriteRenderer spriteRenderer;
    RoomObjectData objectData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (!objectData.IsActive) return;
        Collider2D player = Physics2D.OverlapCircle(transform.position, 5, LayerMask.GetMask("Player"));

        if (!player) return;
        player.GetComponent<Health>().Heal(Random.Range(10, 30));
        objectData.IsActive = false;
        animator.CrossFade("Empty", 0, 0);
    }

    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        return new(true);
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        objectData = roomObjectData;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = objectData.IsActive ? activeSprite : inactiveSprite;
    }
}