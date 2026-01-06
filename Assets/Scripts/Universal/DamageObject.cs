using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class DamageObject : MonoBehaviour
{
    [SerializeField] int criticalHitFontSize, speed, criticalHitSpeed, lifeTime;
    [SerializeField] Color physicalDamageColor, fireDamageColor, waterDamageColor, windDamageColor, earthDamageColor, electricDamageColor, natureDamageColor, iceDamageColor, healingColor;

    float currentLifetime;

    TextMeshPro tmp;
    RectTransform rectTransform;
    Vector2 direction;

    void Awake()
    {
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
        currentLifetime -= Time.deltaTime;
        rectTransform.localScale = Vector3.one * currentLifetime;
        transform.position -= (Vector3)direction.normalized * Time.deltaTime * speed * lifeTime;
        if (currentLifetime > 0) return;

        Destroy(gameObject);
    }

    public void Instantiate(int amount, bool isHeal, Vector2 direction, Element element)
    {
        GetComponent<TextMeshPro>().SetText(Mathf.Abs(amount).ToString());
        this.direction = direction;
        currentLifetime = lifeTime;

        Color color = new ();
        switch (element)
        {
            case Element.Physical: color = physicalDamageColor; break;
            case Element.Fire: color = fireDamageColor; break;
            case Element.Water: color = waterDamageColor; break;
            case Element.Wind: color = windDamageColor; break;
            case Element.Earth: color = earthDamageColor; break;
            case Element.Electric: color = electricDamageColor; break;
            case Element.Nature: color = natureDamageColor; break;
        }
        tmp.color = isHeal ? healingColor : color;
    }
}

public enum DamageType { None, CriticalHit, Heal }