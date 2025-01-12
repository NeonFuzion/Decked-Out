using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    float lifeTime;
    int speed;

    TextMeshPro tmp;
    RectTransform rectTransform;
    Vector2 direction;

    void Awake()
    {
        speed = 5;
        lifeTime = 1;

        tmp = GetComponent<TextMeshPro>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        rectTransform.localScale = Vector3.one * lifeTime;
        transform.position -= (Vector3)direction.normalized * Time.deltaTime * speed * lifeTime;
        if (lifeTime > 0) return;

        Destroy(gameObject);
    }

    public void Instantiate(int amount, Vector2 direction)
    {
        GetComponent<TextMeshPro>().SetText(Mathf.Abs(amount).ToString());
        this.direction = direction;
    }
}
