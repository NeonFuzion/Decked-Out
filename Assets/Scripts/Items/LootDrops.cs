using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrops : MonoBehaviour
{
    [SerializeField] GameObject prefabItemObject;
    [SerializeField] List<Item> commonDrops, rareDrops;

    public List<Item> CommonDrops { get => commonDrops; set => commonDrops = value; }
    public List<Item> RareDrops { get => rareDrops; set => rareDrops = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnDrops()
    {
        if (commonDrops.Count == 0) return;
        Vector2 offset = transform.position;
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Item drop = commonDrops[Random.Range(0, commonDrops.Count)];

            for (int j = 0; j < (drop as Equipment ? 1 : Random.Range(1, 3)); j++)
            {
                offset += new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                GameObject dropObj = Instantiate(prefabItemObject, transform.position, Quaternion.identity);
                dropObj.GetComponent<ItemObject>().Instantiate(drop);
            }
        }
    }

    public void SpawnSpecialDrops()
    {
        int rand = Random.Range(-3, rareDrops.Count);
        if (rand < 0) return;
        Instantiate(rareDrops[rand]);
    }
}
