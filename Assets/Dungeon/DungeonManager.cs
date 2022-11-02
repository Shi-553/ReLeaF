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
    Tilemap objectTilemap;
    [SerializeField]
    Tilemap wallTilemap;

    [SerializeField]
    TerrainTile wetSandTile;
    [SerializeField]
    TerrainTile grassTile;
    [SerializeField]
    TerrainTile messyTile;
    [SerializeField]
    TerrainTile sandTile;
    [SerializeField]
    TerrainTile[] seedTiles;

    [SerializeField]
    Grid grid;

    [SerializeField]
    float growGrassTime = 0.5f;

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

    public void SowGrass(Vector3 worldPos)
    {
        var pos = grid.WorldToCell(worldPos);

        var tile = groundTilemap.GetTile<TerrainTile>(pos);
        if (tile == null)
        {
            return;
        }
        if (!tile.canSowGrass)
        {
            return;
        }
        StartCoroutine(GrowGrass(pos));
    }
    IEnumerator GrowGrass(Vector3Int tilePos)
    {
        groundTilemap.SetTile(tilePos, wetSandTile);

        // ëêÇ™ê∂Ç¶ÇÈÇ‹Ç≈
        yield return new WaitForSeconds(growGrassTime);

        var stackedPos1 = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 1);

        groundTilemap.SetTile(stackedPos1, grassTile);

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
        if (!tile.canSowGrass)
        {
            return;
        }
        groundTilemap.SetTile(pos, seedTiles[(int)type]);
    }


    IEnumerator CureMessy(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(messyCuredTime);
        groundTilemap.SetTile(tilePos, sandTile);
    }

    static public readonly Vector3 CELL_SIZE = new Vector3(0.5f, 0.5f, 0);
}
