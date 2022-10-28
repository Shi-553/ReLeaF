using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public enum SeedType
{
    None = -1,
    Normal,
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
    GameObject[] fruits;
    [SerializeField]
    TerrainTile wetSandTile;
    [SerializeField]
    TerrainTile grassTile;
    [SerializeField]
    TerrainTile messyTile;
    [SerializeField]
    TerrainTile sandTile;
    [SerializeField]
    TerrainTile treeTile;

    [SerializeField]
    Grid grid;

    [SerializeField]
    float growGrassTime = 0.5f;
    [SerializeField]
    float growFruitTime = 0.5f;
    [SerializeField]
    float untilWithersTime = 10.0f;

    [SerializeField]
    float messyCuredTime = 5.0f;

    static readonly public int TILE_STACK_MAX = 3;

    TerrainTile[] tilesBuffer = new TerrainTile[TILE_STACK_MAX];
    Vector3Int[] possBuffer = new Vector3Int[TILE_STACK_MAX];


    Dictionary<Vector3Int, Fruit> fruitDic = new Dictionary<Vector3Int, Fruit>();

    public Fruit Harvest(Vector3 worldPos)
    {
        fruitDic.Remove(grid.WorldToCell(worldPos), out var fruit);
        return fruit;
    }

    private void Start()
    {
        fruitDic.Clear();
    }

    public void SowSeed(Vector3 worldPos, SeedType type)
    {
        if (type < 0 || fruits.Length <= (int)type)
        {
            return;
        }
        var pos = grid.WorldToCell(worldPos);

        // z����̑S�Ẵ^�C��������
        int count = groundTilemap.GetTilesRangeNonAlloc(pos, pos + new Vector3Int(0, 0, TILE_STACK_MAX - 1), possBuffer, tilesBuffer);

        // �ǂꂩ�����񂶂Ⴂ���Ȃ���Ȃ�messyTile�����ɂ���
        if (tilesBuffer.Any(t => t != null && !t.canStepOn))
        {
            for (int i = 0; i < count; i++)
            {
                groundTilemap.SetTile(possBuffer[i], null);
                tilesBuffer[i] = null;
            }
            groundTilemap.SetTile(pos, messyTile);
            StartCoroutine(CureMessy(pos));

            Debug.Log("Messy");

            // �t���[�c���Ȃ���
            if (fruitDic.Remove(pos, out var fruit))
            {
                fruit.SteppedOn();
            }
            return;
        }


        var tile = groundTilemap.GetTile<TerrainTile>(pos);
        if (tile == null)
        {
            return;
        }
        if (!tile.canSowSeed)
        {
            return;
        }
        StartCoroutine(GrowGreenTile(pos, type));
    }

    IEnumerator GrowGreenTile(Vector3Int tilePos, SeedType type)
    {
        groundTilemap.SetTile(tilePos, wetSandTile);
        Debug.Log("wetSand");

        // ����������܂�
        yield return new WaitForSeconds(growGrassTime);

        var stackedPos1 = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 1);
        var stackedPos2 = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 2);

        groundTilemap.SetTile(stackedPos1, grassTile);
        Debug.Log("grass");

        // ����t����܂�
        yield return new WaitForSeconds(growFruitTime);

        if (groundTilemap.GetTile<TerrainTile>(stackedPos1) != grassTile)
        {
            yield break;
        }
        Debug.Log("fruit");


        var fruitObj = Instantiate(fruits[(int)type], grid.CellToWorld(tilePos) + groundTilemap.cellSize / 2, Quaternion.identity);
        var fruit = fruitObj.GetComponent<Fruit>();
        fruitDic.Add(tilePos, fruit);

        // ���S�ɐ�������܂�
        yield return new WaitForSeconds(untilWithersTime);

        groundTilemap.SetTile(stackedPos1, null);



        if (fruit != null && fruitDic.Remove(tilePos))
        {
            // �܂����n����ĂȂ��̂Ŗ؂ɂ���
            fruit.SteppedOn();
            groundTilemap.SetTile(stackedPos2, treeTile);
        }
        else
        {
            if (groundTilemap.GetTile<TerrainTile>(tilePos) == wetSandTile)
            {
                groundTilemap.SetTile(tilePos,sandTile);
            }
        }
    }
    IEnumerator CureMessy(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(messyCuredTime);
        groundTilemap.SetTile(tilePos, sandTile);
        Debug.Log("CureMessy");
    }

    static public readonly Vector3 CELL_SIZE = new Vector3(0.5f, 0.5f, 0);
}
