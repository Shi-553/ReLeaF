using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public enum PlantType
{
    None = -1,
    Foundation,
    Tree,
    Shrub,
    Flower,
    Max
};
public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    Tilemap groundTilemap;

    [SerializeField]
    TerrainTile messyTile;
    [SerializeField]
    TerrainTile sandTile;
    [SerializeField]
    TerrainTile[] seedTiles;

    [SerializeField]
    Grid grid;

    [SerializeField]
    float messyCuredTime = 5.0f;

    static readonly public int TILE_STACK_MAX = 3;

    TerrainTile[] tilesBuffer = new TerrainTile[TILE_STACK_MAX];
    Vector3Int[] possBuffer = new Vector3Int[TILE_STACK_MAX];

    public static DungeonManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public Vector3Int WorldToTilePos(Vector3 worldPos)
    {
        return grid.WorldToCell(worldPos);
    }
    public Vector3 TilePosToWorld(Vector3Int tilePos)
    {
        return grid.CellToWorld(tilePos) + new Vector3(CELL_SIZE, CELL_SIZE) / 2;
    }
    public GameObject GetGroundTileObject(Vector3Int tilePos)
    {
        return groundTilemap.GetInstantiatedObject(tilePos);
    }
    public void Messy(Plant plant)
    {
        var pos = plant.TilePos;
        // zé≤è„ÇÃëSÇƒÇÃÉ^ÉCÉãÇå©ÇÈ
        int count = groundTilemap.GetTilesRangeNonAlloc(pos, pos + new Vector3Int(0, 0, TILE_STACK_MAX - 1), possBuffer, tilesBuffer);


        for (int i = 0; i < count; i++)
        {
            groundTilemap.SetTile(possBuffer[i], null);
            tilesBuffer[i] = null;
        }

        groundTilemap.SetTile(pos, messyTile);
        StartCoroutine(CureMessy(pos));

    }

    public void SowSeed(Vector3 worldPos, PlantType type)
    {
        if (type < 0 || seedTiles.Length <= (int)type)
        {
            return;
        }
        var pos = grid.WorldToCell(worldPos);

        var tile = groundTilemap.GetTile<TerrainTile>(pos);
        if (tile == null)
        {
            return;
        }
        if (type == PlantType.Foundation && !tile.canSowGrass)
        {
            return;
        }
        if (type != PlantType.Foundation){
            if (tile.tileType != TileType.Foundation)
            {
                return;
            }
        }
        groundTilemap.SetTile(pos, seedTiles[(int)type]);
    }


    IEnumerator CureMessy(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(messyCuredTime);
        groundTilemap.SetTile(tilePos, sandTile);
    }

    static public readonly float CELL_SIZE = 0.5f;
}
