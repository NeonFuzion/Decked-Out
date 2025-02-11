using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class DungeonRoomLayout : ScriptableObject
{
    [SerializeField] List<TileInfo> floorTiles, wallTiles, northExitTiles, eastExitTiles, southExitTiles, westExitTiles;
    [SerializeField] List<Vector2> enemySpawnPositions, roomTransitionPositions;
    [SerializeField] List<PrefabPositionPair> specialObjectPositions;
    [SerializeField] Vector2 chestPosition;

    public List<TileInfo> FloorTiles { get => floorTiles; set => floorTiles = value; }
    public List<TileInfo> WallTiles { get => wallTiles; set => wallTiles = value; }
    public List<TileInfo> NorthExitTiles { get => northExitTiles; set => northExitTiles = value; }
    public List<TileInfo> EastExitTiles { get => eastExitTiles; set => eastExitTiles = value; }
    public List<TileInfo> SouthExitTiles { get => southExitTiles; set => southExitTiles = value; }
    public List<TileInfo> WestExitTiles { get => westExitTiles; set => westExitTiles = value; }
    public List<Vector2> EnemySpawnPositions { get => enemySpawnPositions; set => enemySpawnPositions = value; }
    public List<Vector2> RoomTransitionPositions { get => roomTransitionPositions; set => roomTransitionPositions = value; }
    public List<PrefabPositionPair> SpecialObjectPositions { get => specialObjectPositions; set => specialObjectPositions = value; }
    public Vector2 ChestPosition { get => chestPosition; set => chestPosition = value; }
}
