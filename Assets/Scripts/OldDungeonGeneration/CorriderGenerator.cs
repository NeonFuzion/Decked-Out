using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CorriderGenerator : RandomWalkDungeonGenerator
{
    [SerializeField] int corridorLen = 14, corridorCount = 5;
    [SerializeField][Range(0, 1)] float roomPercent = 0.8f;
    [SerializeField] UnityEvent<List<Vector2Int>> onRoomsCreated;

    protected override void RunProceduralGeneration()
    {
        CorridorGeneration();
    }

    void CorridorGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions, deadEnds);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrushTo3x3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    List<Vector2Int> IncreaseCorridorBrushTo3x3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int prevDir = Vector2Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int dirFromCell = corridor[i] - corridor[i - 1];
            if (prevDir != Vector2Int.zero && dirFromCell != prevDir)
            {
                for (int x = -1; x < -2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                prevDir = dirFromCell;
            }
            else
            {
                Vector2Int corridorOffset = Direction2D.RotateCompass(dirFromCell, -1);
                Debug.Log(corridorOffset + " | " + dirFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + corridorOffset);
            }
        }
        return newCorridor;
    }

    List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int curPos = startPos;
        potentialRoomPositions.Add(curPos);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        
        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(curPos, corridorLen);
            corridors.Add(corridor);
            curPos = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(curPos);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }

    public List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (Vector2Int floorPos in floorPositions)
        {
            int neighborCount = 0;
            foreach (Vector2Int dir in Direction2D.CardinalDirections)
            {
                if (floorPositions.Contains(floorPos + dir)) neighborCount++;
            }
            if (neighborCount == 1) deadEnds.Add(floorPos);
        }
        return deadEnds;
    }

    HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions, List<Vector2Int> deadEnds)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        HashSet<Vector2Int> randRooms = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).ToHashSet();
        onRoomsCreated.Invoke(randRooms.ToList());
        HashSet<Vector2Int> rooms = deadEnds.ToHashSet();
        rooms.UnionWith(randRooms);
        List<Vector2Int> roomsToCreate = rooms.Take(roomToCreateCount).ToList();

        foreach (Vector2Int roomPos in roomsToCreate)
        {
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randWalkParams, roomPos);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }
}
