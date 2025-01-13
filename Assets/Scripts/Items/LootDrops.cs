using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrops : MonoBehaviour
{
    [SerializeField] GameObject prefabItemObject;
    [SerializeField] List<Item> commonDrops, rareDrops, singleDrops;

    public List<Item> CommonDrops { get => commonDrops; set => commonDrops = value; }
    public List<Item> RareDrops { get => rareDrops; set => rareDrops = value; }
    public List<Item> SingleDrops { get => singleDrops; set => singleDrops = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateItemDrop(Item drop)
    {
        Vector2 offset = (Vector2)transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        GameObject dropObj = Instantiate(prefabItemObject, offset, Quaternion.identity);
        dropObj.GetComponent<ItemObject>().Instantiate(drop);
    }

    public void SpawnDrops()
    {
        SpawnCommonDrops();
        SpawnSingleDrops();
        SpawnRareDrops();
    }

    public void SpawnCommonDrops()
    {
        if (commonDrops.Count == 0) return;
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Item drop = commonDrops[Random.Range(0, commonDrops.Count)];

            for (int j = 0; j < (drop as Equipment ? 1 : Random.Range(1, 3)); j++)
            {
                CreateItemDrop(drop);
            }
        }
    }

    public void SpawnSingleDrops()
    {
        if (singleDrops.Count == 0) return;
        int rand = Random.Range(0, singleDrops.Count);
        CreateItemDrop(singleDrops[rand]);
    }

    public void SpawnRareDrops()
    {
        if (rareDrops.Count == 0) return;
        int rand = Random.Range(-3, rareDrops.Count);
        if (rand < 0) return;
        CreateItemDrop(rareDrops[rand]);
    }
}
