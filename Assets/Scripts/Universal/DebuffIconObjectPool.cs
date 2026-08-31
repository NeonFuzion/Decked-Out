using System.Collections.Generic;
using UnityEngine;

public class DebuffIconObjectPool : MonoBehaviour
{
    [SerializeField] int startingAmount = 20;
    [SerializeField] Transform poolParent;
    [SerializeField] GameObject PrefabDebuffIcon;

    public static DebuffIconObjectPool Instance;

    List<GameObject> items;

    void Awake()
    {
        if (Instance) return;
        Instance = this;

        items = new ();
        for (int i = 0; i < startingAmount; i++)
        {
            CreateNewIcon();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject CreateNewIcon()
    {
        GameObject icon = Instantiate(PrefabDebuffIcon, poolParent);
        icon.SetActive(false);
        items.Add(icon);
        return icon;
    }

    public GameObject RetrieveItem()
    {
        GameObject item = items.Find(item => item.gameObject.activeInHierarchy) ?? CreateNewIcon();
        item.SetActive(true);
        return item;
    }

    public void ReturnItem(GameObject item)
    {
        item.SetActive(false);
    }
}
