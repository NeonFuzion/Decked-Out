using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> wallPositions = FindWallsInDirection(floorPositions, Direction2D.EightDirections);

        foreach (Vector2Int pos in wallPositions)
        {
            tilemapVisualizer.PaintSingleWall(pos);
        }
    }

    public static HashSet<Vector2Int> FindWallsInDirection(HashSet<Vector2Int> floorPositions, List<Vector2Int> dirList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            foreach (Vector2Int dir in dirList)
            {
                Vector2Int neighborPos = pos + dir;
                if (floorPositions.Contains(neighborPos)) continue;
                wallPositions.Add(neighborPos);
            }
        }
        return wallPositions;
    }
}
