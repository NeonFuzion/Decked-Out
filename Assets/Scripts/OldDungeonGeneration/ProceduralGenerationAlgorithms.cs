using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPos, int length)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPos);
        Vector2Int prevPos = startPos;
        for (int i = 0; i < length; i++)
        {
            Vector2Int newPos = prevPos + Direction2D.GetRandomCardinalDirection();
            path.Add(newPos);
            prevPos = newPos;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos, int corridorLen)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        List<Vector2Int> dirList = Direction2D.CardinalDirections;
        Vector2Int direction = Direction2D.GetRandomCardinalDirection();
        Vector2Int curPos = startPos;
        corridor.Add(curPos);
        for (int j = 0; j < corridorLen; j++)
        {
            curPos += direction;
            corridor.Add(curPos);
        }
        return corridor;
    }
}

public static class Direction2D
{
    static List<Vector2Int> cardinalDirections = new List<Vector2Int>()
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    static List<Vector2Int> diagonalDirections = new List<Vector2Int>()
    {
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1)
    };

    static List<Vector2Int> eightDirections = new List<Vector2Int>()
    {
        Vector2Int.up, new Vector2Int(1, 1), Vector2Int.right, new Vector2Int(1, -1),
        Vector2Int.down, new Vector2Int(-1, -1), Vector2Int.left, new Vector2Int(-1, 1)
    };

    public static List<Vector2Int> CardinalDirections { get => cardinalDirections; }

    public static List<Vector2Int> DiagonalDirections { get => diagonalDirections; }

    public static List<Vector2Int> EightDirections { get => eightDirections; }

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirections[Random.Range(0, cardinalDirections.Count)];
    }

    public static Vector2Int RotateCompass(Vector2Int direction, int amount)
    {
        int turns = turns = amount > 0 ? amount : cardinalDirections.Count - amount;
        return cardinalDirections[(turns + cardinalDirections.IndexOf(direction)) % cardinalDirections.Count];
    }
}
