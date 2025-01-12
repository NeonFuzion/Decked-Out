using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class DungeonCreator : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] Tilemap wallTilemap, floorTilemap, northExitTilemap, eastExitTilemap, southExitTilemap, westExitTilemap;
    [SerializeField] GameObject[] roomTransitions, enemySpawners;
    [SerializeField] GameObject chest;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<TileInfo> FindTiles(Tilemap tilemap)
    {
        List<TileInfo> tileInfo = new List<TileInfo>();
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y);
                TileBase tile = tilemap.GetTile(position);
                Quaternion rotation = tilemap.GetTransformMatrix(position).rotation;
                if (!tile) continue;

                tileInfo.Add(new TileInfo(tile, position, rotation));
            }
        }
        return tileInfo;
    }

    public void SaveLayout()
    {
        DungeonRoomLayout roomLayout = roomPrefab.GetComponent<DungeonRoomLayout>();
        if (chest) roomLayout.ChestPosition = chest.transform.position;

        roomLayout.FloorTiles = FindTiles(floorTilemap);
        roomLayout.WallTiles = FindTiles(wallTilemap);
        roomLayout.NorthExitTiles = FindTiles(northExitTilemap);
        roomLayout.EastExitTiles = FindTiles(eastExitTilemap);
        roomLayout.SouthExitTiles = FindTiles(southExitTilemap);
        roomLayout.WestExitTiles = FindTiles(westExitTilemap);

        roomLayout.EnemySpawnPositions = enemySpawners.Select(gm => (Vector2)gm.transform.position).ToList();
        roomLayout.RoomTransitionPositions = roomTransitions.Select(gm => (Vector2)gm.transform.position).ToList();
    }
}

[Serializable]
public class TileInfo
{
    [SerializeField] TileBase tile;
    [SerializeField] Vector3Int position;
    [SerializeField] Quaternion rotation;

    public TileBase Tile { get => tile; }
    public Vector3Int Position { get => position; }
    public Quaternion Rotation { get => rotation; }

    public TileInfo(TileBase tile, Vector3Int position, Quaternion rotation)
    {
        this.tile = tile;
        this.position = position;
        this.rotation = rotation;
    }
}