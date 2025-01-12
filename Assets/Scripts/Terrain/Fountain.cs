using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    int roomCountdown;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        roomCountdown = 0;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if (roomCountdown > 0) return;
        Collider2D player = Physics2D.OverlapCircle(transform.position, 5, LayerMask.GetMask("Player"));

        if (!player) return;
        roomCountdown = 3;
        player.GetComponent<Health>().Heal(Random.Range(10, 30));
        animator.CrossFade("Empty", 0, 0);
    }

    public void AddRoomCount()
    {
        roomCountdown--;
    }
}
