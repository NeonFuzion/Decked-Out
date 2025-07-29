using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;

public class DungeonCreator : MonoBehaviour
{
    [SerializeField] DungeonRoomLayout roomLayout;
    [SerializeField] Tilemap wallTilemap, floorTilemap, northExitTilemap, southExitTilemap, eastExitTilemap, westExitTilemap;
    [SerializeField] GameObject[] roomTransitions;
    [SerializeField] GameObject roomObjectParent;

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
                if (!tile) continue;

                tileInfo.Add(new TileInfo(tile, position));
            }
        }
        return tileInfo;
    }

    void PlaceTiles(List<TileInfo> tiles, Tilemap tilemap)
    {
        tilemap.ClearAllTiles();
        if (tiles == null) return;
        foreach (TileInfo tile in tiles)
        {
            tilemap.SetTile(tile.Position, tile.Tile);
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
        roomLayout.FloorTiles = FindTiles(floorTilemap);
        roomLayout.WallTiles = FindTiles(wallTilemap);
        roomLayout.NorthExitTiles = FindTiles(northExitTilemap);
        roomLayout.EastExitTiles = FindTiles(eastExitTilemap);
        roomLayout.SouthExitTiles = FindTiles(southExitTilemap);
        roomLayout.WestExitTiles = FindTiles(westExitTilemap);

        roomLayout.RoomTransitionPositions = GetPositions(roomTransitions);

        List<PrefabPositionPair> roomObjectPositions = new List<PrefabPositionPair>();
        for (int i = 0; i < roomObjectParent.transform.childCount; i++)
        {
            Transform roomObject = roomObjectParent.transform.GetChild(i);
            GameObject prefabRoomObject = roomObject.gameObject;
            prefabRoomObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefabRoomObject).gameObject;
            roomObjectPositions.Add(new PrefabPositionPair(prefabRoomObject, roomObject.position));
        }
        roomLayout.RoomObjectPositions = roomObjectPositions;

        EditorUtility.SetDirty(roomLayout);
        AssetDatabase.SaveAssets();
    }

    public void LoadLayout()
    {
        PlaceTiles(roomLayout.FloorTiles, floorTilemap);
        PlaceTiles(roomLayout.WallTiles, wallTilemap);
        PlaceTiles(roomLayout.NorthExitTiles, northExitTilemap);
        PlaceTiles(roomLayout.EastExitTiles, eastExitTilemap);
        PlaceTiles(roomLayout.SouthExitTiles, southExitTilemap);
        PlaceTiles(roomLayout.WestExitTiles, westExitTilemap);

        for (int i = 0; i < roomLayout.RoomTransitionPositions.Count; i++)
        {
            roomTransitions[i].transform.position = roomLayout.RoomTransitionPositions[i];
        }

        while (roomObjectParent.transform.childCount > 0)
        {
            DestroyImmediate(roomObjectParent.transform.GetChild(0).gameObject);
        }
        foreach (PrefabPositionPair pair in roomLayout.RoomObjectPositions)
        {
            GameObject specialObject = Instantiate(pair.Prefab, pair.Position, Quaternion.identity);
            PrefabUtility.ConvertToPrefabInstance(specialObject, pair.Prefab, new ConvertToPrefabInstanceSettings(), InteractionMode.UserAction);
            specialObject.transform.SetParent(roomObjectParent.transform, true);
        }
    }
}
#endif

[Serializable]
public class TileInfo
{
    [SerializeField] TileBase tile;
    [SerializeField] Vector3Int position;

    public TileBase Tile { get => tile; }
    public Vector3Int Position { get => position; }

    public TileInfo(TileBase tile, Vector3Int position)
    {
        this.tile = tile;
        this.position = position;
    }
}