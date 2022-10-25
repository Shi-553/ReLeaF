using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorInfo
{

};
public class PatternInfo : MonoBehaviour
{

   public BoundsInt GetBounds()
    {
        return GetComponentInChildren<Tilemap>().cellBounds;
    }
    //public DoorInfo[] GetDoors()
    //{
    //    List<TerrainTile> processedTiles = new();
    //    var tileMap = GetComponentInChildren<Tilemap>();
    //    foreach (var pos in tileMap.cellBounds.allPositionsWithin)
    //    {
    //        var tile = tileMap.GetTile<TerrainTile>(pos);
    //        if (tile.type == TileType.Door)
    //        {

    //        }
    //    }
    //}
}
