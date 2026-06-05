using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DungeonRoomLayout : ScriptableObject
{
    [SerializeField] Sprite roomIcon;
    [SerializeField] List<TileInfo> floorTiles, wallTiles, northExitTiles, eastExitTiles, southExitTiles, westExitTiles;
    [SerializeField] List<Vector2> roomTransitionPositions;
    [SerializeField] List<PrefabPositionPair> roomObjectPositions;

    public Sprite RoomIcon { get => roomIcon; set => roomIcon = value; }
    public List<TileInfo> FloorTiles { get => floorTiles; set => floorTiles = value; }
    public List<TileInfo> WallTiles { get => wallTiles; set => wallTiles = value; }
    public List<TileInfo> NorthExitTiles { get => northExitTiles; set => northExitTiles = value; }
    public List<TileInfo> EastExitTiles { get => eastExitTiles; set => eastExitTiles = value; }
    public List<TileInfo> SouthExitTiles { get => southExitTiles; set => southExitTiles = value; }
    public List<TileInfo> WestExitTiles { get => westExitTiles; set => westExitTiles = value; }
    public List<Vector2> RoomTransitionPositions { get => roomTransitionPositions; set => roomTransitionPositions = value; }
    public List<PrefabPositionPair> RoomObjectPositions { get => roomObjectPositions; set => roomObjectPositions = value; }
}
