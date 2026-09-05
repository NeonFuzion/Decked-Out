using System.Collections.Generic;
using UnityEngine;

public class HealthBarObjectPool : MonoBehaviour
{
    public static HealthBarObjectPool Instance;

    [SerializeField] GameObject prefabHealthBar;
    [SerializeField] Transform canvas;

    List<HealthBar> healthBars;

    void Awake()
    {
        if (Instance) return;
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBars = new ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HealthBar RetrieveHealthBar(Transform target, Health healthScript, bool isMoving, Stagger stagger = null, DebuffManager debuffManager = null)
    {
        HealthBar healthBar = healthBars.Find(bar => !bar.gameObject.activeInHierarchy);
        
        if (!healthBar)
        {
            healthBar = Instantiate(prefabHealthBar, canvas).GetComponent<HealthBar>();
            healthBars.Add(healthBar);
        }

        StaggerBar staggerBar = healthBar.GetComponent<StaggerBar>();
        staggerBar.Initialize();
        stagger?.SetStaggerBar(staggerBar);

        debuffManager?.SetDebuffBar(healthBar.GetComponent<DebuffBar>());

        healthScript.OnHealthChanged.AddListener(healthBar.SetFill);
        healthBar.transform.position = target.position;
        healthBar.Initialize(isMoving ? target : null);
        healthBar.gameObject.SetActive(true);
        return healthBar;
    }

    public void ReturnHealthBar(HealthBar healthBar, Health health, Stagger stagger = null)
    {
        health.OnHealthChanged?.RemoveListener(healthBar.SetFill);
        stagger?.SetStaggerBar(null);
        healthBar.gameObject.SetActive(false);
    }
}
