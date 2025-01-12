using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] Tilemap floorTilemap, wallTimemap;
    [SerializeField] TileBase wallTile, floorTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap);
    }

    void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, floorTile, position);
        }
    }

    void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePos = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePos, tile);
    }

    public void PaintSingleWall(Vector2Int pos)
    {
        PaintSingleTile(wallTimemap, wallTile, pos);
    }

    public void Clear()
    {
        wallTimemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
    }
}
