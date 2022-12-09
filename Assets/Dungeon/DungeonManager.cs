using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    [ClassSummary("タイルマップマネージャー")]
    public class DungeonManager : SingletonBase<DungeonManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        List<TerrainTile> terrainTiles = new();

        [SerializeField]
        Tilemap groundTilemap;

        Dictionary<TileType, TerrainTile> terrainTileDic;

        public Dictionary<Vector2Int, TileObject> tiles = new Dictionary<Vector2Int, TileObject>();

        [field: SerializeField, ReadOnly]
        public int MaxGreeningCount { get; private set; }

        public readonly struct TileChangedInfo
        {
            readonly public Vector2Int tilePos;
            readonly public TileObject beforeTile;
            readonly public TileObject afterTile;

            public TileChangedInfo(Vector2Int tilePos, TileObject beforeTile, TileObject afterTile)
            {
                this.tilePos = tilePos;
                this.beforeTile = beforeTile;
                this.afterTile = afterTile;
            }
        }
        public event Action<TileChangedInfo> OnTileChanged;

        protected override void Init()
        {
            foreach (var pos in groundTilemap.cellBounds.allPositionsWithin)
            {
                var tile = groundTilemap.GetTile<TerrainTile>(pos);
                if (tile != null && tile.CurrentTileObject.CanGreening(false))
                {
                    MaxGreeningCount++;
                }
            }
            terrainTileDic = terrainTiles.ToDictionary(t => t.CurrentTileObject.TileType, t => t);

            foreach (var terrainTile in terrainTileDic)
            {
                terrainTile.Value.Init();
            }
        }

        public Vector2Int WorldToTilePos(Vector3 worldPos)
        {
            return (Vector2Int)groundTilemap.WorldToCell(worldPos);
        }
        public Vector2 TilePosToWorld(Vector2Int tilePos)
        {
            return (Vector2)groundTilemap.CellToWorld((Vector3Int)tilePos) + new Vector2(CELL_SIZE, CELL_SIZE) / 2;
        }
        public Vector2 TilePosToWorld(Vector2 tilePos)
        {
            var floor = Vector2Int.FloorToInt(tilePos);
            var smallNumber = tilePos - floor;
            return (Vector2)groundTilemap.CellToWorld((Vector3Int)floor) + (smallNumber * CELL_SIZE) + new Vector2(CELL_SIZE, CELL_SIZE) / 2;
        }
        public bool TryGetTile(Vector2Int pos, out TileObject tile) => tiles.TryGetValue(pos, out tile);
        public TileObject GetTile(Vector2Int pos) => tiles.GetValueOrDefault(pos, null);


        public bool TryGetTile<T>(Vector2Int pos, out T tile) where T : TileObject
        {
            tiles.TryGetValue(pos, out var tileBase);
            if (tileBase is T t)
            {
                tile = t;
                return true;
            }
            tile = null;
            return false;
        }
        public TileObject GetTile<T>(Vector2Int pos) where T : TileObject => tiles.GetValueOrDefault(pos, null) as T;

        public bool SowSeed(Vector2Int tilePos, bool isSpecial = false)
        {
            if (!TryGetTile(tilePos, out var tile) || !tile.CanGreening(isSpecial))
            {
                return false;
            }
            if (tile is SpawnLake lake)
            {
                lake.Greening();
                return true;
            }
            terrainTileDic[TileType.Plant].IsInvincible = false;
            ChangeTile(tilePos, terrainTileDic[TileType.Plant]);
            return true;
        }

        public bool SowInvincibleSeed(Vector2Int tilePos)
        {
            if (TryGetTile(tilePos, out var tile) && tile is SpawnLake lake)
            {
                lake.Greening();
                return true;
            }
            terrainTileDic[TileType.Plant].IsInvincible = true;

            ChangeTile(tilePos, terrainTileDic[TileType.Plant]);
            return true;
        }

        void ChangeTile(Vector2Int pos, TerrainTile after)
        {
            if (tiles.Remove(pos, out var before))
                before.Release();
            else
            {
                before = TileObject.NullTile;
            }
            OnTileChanged?.Invoke(new TileChangedInfo(pos, before, after.CurrentTileObject));
            groundTilemap.SetTile((Vector3Int)pos, after);

        }


        public void Messy(Vector2Int tilePos, IMultipleVisual visual)
        {
            var messy = terrainTileDic[TileType.Messy] as SelectTile;
            messy.Selected = visual;
            ChangeTile(tilePos, messy);
        }
        public void ToSand(Vector2Int tilePos)
        {
            ChangeTile(tilePos, terrainTileDic[TileType.Sand]);
        }
        public void ToEnemyPlant(Vector2Int tilePos)
        {
            ChangeTile(tilePos, terrainTileDic[TileType.EnemyPlant]);
        }


        static public readonly float CELL_SIZE = 0.5f;
    }
}