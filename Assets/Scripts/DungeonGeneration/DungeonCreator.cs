using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class DungeonCreator : MonoBehaviour
{
    [SerializeField] DungeonRoomLayout roomLayout;
    [SerializeField] GameObject prefabEnemySpawner;
    [SerializeField] Tilemap wallTilemap, floorTilemap;
    [SerializeField] Tilemap[] exitTilemaps;
    [SerializeField] GameObject[] roomTransitions, enemySpawners;
    [SerializeField] GameObject chest, specialObjectsParent;

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

    void PlaceTiles(List<TileInfo> tiles, Tilemap tilemap)
    {
        tilemap.ClearAllTiles();
        foreach (TileInfo tile in tiles)
        {
            tilemap.SetTile(tile.Position, tile.Tile);
            tilemap.SetTransformMatrix(tile.Position, Matrix4x4.Rotate(tile.Rotation));
        }
    }

    List<Vector2> GetPositions(GameObject[] gameObjects)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject.activeInHierarchy) continue;
            positions.Add(gameObject.transform.position);
        }
        return positions;
    }

    public void SaveLayout()
    {
        if (chest.activeInHierarchy) roomLayout.ChestPosition = chest.transform.position;

        roomLayout.FloorTiles = FindTiles(floorTilemap);
        roomLayout.WallTiles = FindTiles(wallTilemap);
        roomLayout.NorthExitTiles = FindTiles(exitTilemaps[0]);
        roomLayout.EastExitTiles = FindTiles(exitTilemaps[1]);
        roomLayout.SouthExitTiles = FindTiles(exitTilemaps[2]);
        roomLayout.WestExitTiles = FindTiles(exitTilemaps[3]);

        roomLayout.EnemySpawnPositions = GetPositions(enemySpawners);
        roomLayout.RoomTransitionPositions = GetPositions(roomTransitions);

        List<PrefabPositionPair> specialObjectPositions = new List<PrefabPositionPair>();
        for (int i = 0; i < specialObjectsParent.transform.childCount; i++)
        {
            Transform specialObject = specialObjectsParent.transform.GetChild(i);
            if (!specialObject.gameObject.activeInHierarchy) continue;
            specialObjectPositions.Add(new PrefabPositionPair(specialObject.gameObject, specialObject.position));
        }
        roomLayout.SpecialObjectPositions = specialObjectPositions;

        EditorUtility.SetDirty(roomLayout);
        AssetDatabase.SaveAssets();
    }

    public void LoadLayout()
    {
        chest.transform.position = roomLayout.ChestPosition;

        PlaceTiles(roomLayout.FloorTiles, floorTilemap);
        PlaceTiles(roomLayout.WallTiles, wallTilemap);
        PlaceTiles(roomLayout.NorthExitTiles, exitTilemaps[0]);
        PlaceTiles(roomLayout.EastExitTiles, exitTilemaps[1]);
        PlaceTiles(roomLayout.SouthExitTiles, exitTilemaps[2]);
        PlaceTiles(roomLayout.WestExitTiles, exitTilemaps[3]);

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            if (i < roomLayout.EnemySpawnPositions.Count)
            {
                enemySpawners[i].SetActive(true);
                enemySpawners[i].transform.position = roomLayout.EnemySpawnPositions[i];
            }
            else
            {
                enemySpawners[i].SetActive(false);
            }
        }

        for (int i = 0; i < roomLayout.RoomTransitionPositions.Count; i++)
        {
            roomTransitions[i].transform.position = roomLayout.RoomTransitionPositions[i];
        }

        while (specialObjectsParent.transform.childCount > 0)
        {
            Destroy(specialObjectsParent.transform.GetChild(0).gameObject);
        }
        foreach (PrefabPositionPair pair in roomLayout.SpecialObjectPositions)
        {
            GameObject specialObject = Instantiate(pair.Prefab, pair.Position, Quaternion.identity);
            specialObject.transform.SetParent(specialObjectsParent.transform, true);
        }
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