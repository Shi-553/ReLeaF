using DebugLogExtension;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public enum PlantType
    {
        None = -1,
        Foundation,
        Max
    };
    [ClassSummary("タイルマップマネージャー")]
    public class DungeonManager : SingletonBase<DungeonManager>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        Tilemap groundTilemap;

        [SerializeField]
        TerrainTile sandTile;
        [SerializeField]
        TerrainTile[] seedTiles;

        [SerializeField]
        SelectTile messyTile;

        [SerializeField]
        TerrainTile enemyPlant;


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
                if (tile != null && tile.CurrentTileObject.CanSowGrass(false))
                {
                    MaxGreeningCount++;
                }
            }

            sandTile.Init();
            seedTiles.ForEach(t => t.Init());
            messyTile.Init();
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

        public bool CanSowSeed(TileObject tile, PlantType type, bool isSpecial)
        {
            if (type < 0 || seedTiles.Length <= (int)type)
            {
                return false;
            }

            if (type == PlantType.Foundation && !tile.CanSowGrass(isSpecial))
            {
                return false;
            }

            return true;
        }
        public bool CanSowSeed(Vector2Int tilePos, PlantType type, bool isSpecial)
        {
            if (TryGetTile(tilePos, out var tile))
                return CanSowSeed(tile, type, isSpecial);
            return false;
        }

        public bool SowSeed(Vector2Int tilePos, PlantType type, bool isSpecial = false)
        {
            if (!CanSowSeed(tilePos, type, isSpecial))
            {
                return false;
            }
            seedTiles[(int)type].IsInvincible = false;
            ChangeTile(tilePos, seedTiles[(int)type]);
            return true;
        }

        public bool SowInvincibleSeed(Vector2Int tilePos, PlantType type)
        {
            seedTiles[(int)type].IsInvincible = true;

            ChangeTile(tilePos, seedTiles[(int)type]);
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
            var af = groundTilemap.GetTile((Vector3Int)pos);
            if (af == null)
                af.DebugLog();
        }


        public void Messy(Vector2Int tilePos, IMultipleVisual visual)
        {
            messyTile.Selected = visual;
            ChangeTile(tilePos, messyTile);
        }
        public void ToSand(Vector2Int tilePos)
        {
            ChangeTile(tilePos, sandTile);
        }
        public void ToEnemyPlant(Vector2Int tilePos)
        {
            ChangeTile(tilePos, enemyPlant);
        }


        static public readonly float CELL_SIZE = 0.5f;
    }
}