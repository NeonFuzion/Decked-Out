using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEfect : MonoBehaviour
{
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public void Initialize(Vector2 position)
    {
        transform.position = position;

        StartCoroutine(DeathTimer());
    }
}
