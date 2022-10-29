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
    int[] treeHP;
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
    float growTreeTime = 10.0f;
    [SerializeField]
    float regrowFruitTime = 0.5f;

    [SerializeField]
    float messyCuredTime = 5.0f;

    static readonly public int TILE_STACK_MAX = 3;

    TerrainTile[] tilesBuffer = new TerrainTile[TILE_STACK_MAX];
    Vector3Int[] possBuffer = new Vector3Int[TILE_STACK_MAX];


    Dictionary<Vector3Int, Fruit> fruitDic = new Dictionary<Vector3Int, Fruit>();

    Transform fruitsParent;
    public Fruit Harvest(Vector3 worldPos)
    {
        var pos = grid.WorldToCell(worldPos);
        if (fruitDic.Remove(pos, out var fruit))
        {
           // StartCoroutine(RegrowFruit(pos, fruit.SeedType));
        }
        return fruit;
    }
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
    private void Start()
    {
        fruitDic.Clear();
        fruitsParent = transform.Find("Fruits");
    }
    public Vector3Int WorldToTilePos(Vector3 worldPos)
    {
        return grid.WorldToCell(worldPos);
    }

    public void Messy(Vector3 worldPos)
    {
        var pos= WorldToTilePos(worldPos);
        // z軸上の全てのタイルを見る
        int count = groundTilemap.GetTilesRangeNonAlloc(pos, pos + new Vector3Int(0, 0, TILE_STACK_MAX - 1), possBuffer, tilesBuffer);

        //どれかが踏んじゃいけないやつならmessyTileだけにする
        //if (tilesBuffer.Any(t => t != null && !t.canStepOn))
        //{
            for (int i = 0; i < count; i++)
            {
                groundTilemap.SetTile(possBuffer[i], null);
                tilesBuffer[i] = null;
            }
            groundTilemap.SetTile(pos, messyTile);
            StartCoroutine(CureMessy(pos));

            Debug.Log("Messy");

            // フルーツをなくす
            if (fruitDic.Remove(pos, out var fruit))
            {
                fruit.SteppedOn();
            }
            return;
       // }
    }
    public void SowSeed(Vector3 worldPos, SeedType type)
    {
        if (type < 0 || fruits.Length <= (int)type)
        {
            return;
        }
        var pos = grid.WorldToCell(worldPos);

        

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

        // 草が生えるまで
        yield return new WaitForSeconds(growGrassTime);

        var stackedPos1 = new Vector3Int(tilePos.x, tilePos.y, tilePos.z + 1);

        groundTilemap.SetTile(stackedPos1, grassTile);
        Debug.Log("grass");


        // 完全に成長するまで
        yield return new WaitForSeconds(growTreeTime);

        if (groundTilemap.GetTile<TerrainTile>(stackedPos1) != grassTile)
        {
            yield break;
        }

        groundTilemap.SetTile(stackedPos1, treeTile);
        Debug.Log("tree");

        var fruitObj = Instantiate(fruits[(int)type], grid.CellToWorld(tilePos) + groundTilemap.cellSize / 2, Quaternion.identity, fruitsParent);
        var fruit = fruitObj.GetComponent<Fruit>();
        fruitDic.Add(tilePos, fruit);

    }

    IEnumerator RegrowFruit(Vector3Int tilePos, SeedType type)
    {
        // もう一度実を付けるまで
        yield return new WaitForSeconds(regrowFruitTime);

        if (groundTilemap.GetTile<TerrainTile>(tilePos) != treeTile)
        {
            yield break;
        }

        var fruitObj = Instantiate(fruits[(int)type], grid.CellToWorld(tilePos) + groundTilemap.cellSize / 2, Quaternion.identity, fruitsParent);
        var fruit = fruitObj.GetComponent<Fruit>();
        fruitDic.Add(tilePos, fruit);



        Debug.Log("fruit");

    }
    IEnumerator CureMessy(Vector3Int tilePos)
    {
        yield return new WaitForSeconds(messyCuredTime);
        groundTilemap.SetTile(tilePos, sandTile);
        Debug.Log("CureMessy");
    }

    static public readonly Vector3 CELL_SIZE = new Vector3(0.5f, 0.5f, 0);
}
