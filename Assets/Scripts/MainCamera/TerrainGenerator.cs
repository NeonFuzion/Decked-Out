using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] Tilemap tilemap, collision;
    [SerializeField] GameObject prefabChest;

    int boundaries, enemyCounter;

    // Start is called before the first frame update
    void Start()
    {
        boundaries = 20;
        enemyCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateObjects(Sprite sprite)
    {
        GameObject terrain = new GameObject("Terrain");
        terrain.AddComponent<BoxCollider2D>().size = sprite.textureRect.size;
        terrain.AddComponent<SpriteRenderer>().sprite = sprite;
    }
}
