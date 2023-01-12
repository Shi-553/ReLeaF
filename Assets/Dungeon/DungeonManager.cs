using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    [Serializable]
    class TileArray
    {
        [SerializeField]
        public TerrainTile[] tiles;

        public void Sort()
        {
            tiles = tiles
                .OrderBy(t => (t.CurrentTileObject as IMultipleVisual).VisualType)
                .ToArray();
        }
    }
    [ClassSummary("タイルマップマネージャー")]
    public class DungeonManager : SingletonBase<DungeonManager>
    {
        public override bool DontDestroyOnLoad => false;


        [SerializeField]
        Tilemap tilemap;

        public Dictionary<Vector2Int, TileObject> tiles = new();

        [field: SerializeField, ReadOnly]
        public int MaxGreeningCount { get; private set; }

        bool CanChangeTile { get; set; } = true;

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

        public readonly struct GreeningInfo
        {
            readonly public Vector2Int tilePos;
            readonly public bool WasAleadyGreening;

            public GreeningInfo(Vector2Int tilePos, bool wasAleadyGreening)
            {
                this.tilePos = tilePos;
                WasAleadyGreening = wasAleadyGreening;
            }
        }
        public event Action<GreeningInfo> OnGreening;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (callByAwake)
            {
                foreach (var pos in tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = tilemap.GetTile<TerrainTile>(pos);
                    if (tile != null && tile.CurrentTileObject.CanGreening(true))
                    {
                        MaxGreeningCount++;
                    }
                }
                tilemap.transform.parent.GetComponentsInChildren<Tilemap>()
                     .ForEach(tm => tm.RefreshAllTiles());

            }
        }
        protected override void UninitBeforeSceneUnload(bool isDestroy)
        {
            CanChangeTile = false;
            foreach (var tile in tiles.Values)
            {
                tile.Release();
                if (tile.HasParent)
                    tile.Parent.Release();
            }
        }


        public Vector2Int WorldToTilePos(Vector3 worldPos)
        {
            return (Vector2Int)tilemap.WorldToCell(worldPos);
        }
        public Vector2 TilePosToWorld(Vector2Int tilePos)
        {
            return (Vector2)tilemap.CellToWorld((Vector3Int)tilePos) + new Vector2(CELL_SIZE, CELL_SIZE) / 2;
        }
        public Vector2 TilePosToWorld(Vector2 tilePos)
        {
            var floor = Vector2Int.FloorToInt(tilePos);
            var smallNumber = tilePos - floor;
            return (Vector2)tilemap.CellToWorld((Vector3Int)floor) + (smallNumber * CELL_SIZE) + new Vector2(CELL_SIZE, CELL_SIZE) / 2;
        }
        public bool TryGetTile(Vector2Int pos, out TileObject tile) => tiles.TryGetValue(pos, out tile);
        public TileObject GetTile(Vector2Int pos) => tiles.GetValueOrDefault(pos, null);


        public bool TryGetTile<T>(Vector2Int pos, out T tile) where T : TileObject
        {
            if (tiles.GetValueOrDefault(pos) is T t)
            {
                tile = t;
                return true;
            }
            tile = null;
            return false;
        }
        public TileObject GetTile<T>(Vector2Int pos) where T : TileObject => tiles.GetValueOrDefault(pos, null) as T;


        public bool SowSeed(Vector2Int tilePos, bool isSpecial = false, bool isInvincible = false)
        {
            if (!TryGetTile(tilePos, out var tile))
            {
                return false;
            }

            if (tile.CanOrAleeadyGreening(isSpecial))
            {
                OnGreening?.Invoke(new GreeningInfo(tilePos, tile.IsAlreadyGreening));

                TileEffectManager.Singleton.SetEffect(TileEffectType.ToLeaf, TilePosToWorld(tilePos));
            }

            if (!tile.CanGreening(isSpecial))
                return false;

            if (tile is SpawnLake lake)
            {
                lake.Greening();
                return true;
            }

            var index = (tile.IsOnlyNormalFoundation || tile.ParentOrThis.IsOnlyNormalFoundation) ? 1 : 0;

            var atfer = ChangeTile(tile.TilePos, TileType.Foundation, index);
            if (atfer != null)
                atfer.IsInvincible = isInvincible;

            return true;
        }


        TileObject ChangeTile(Vector2Int pos, TileType type, int index = 0)
        {
            if (!CanChangeTile || !tiles.TryGetValue(pos, out var before) || before.TileType == type)
            {
                return null;
            }

            tiles.Remove(pos);
            before.Release();

            var after = DungeonTilePalette.Singleton.TerrainTileDic[type][index];
            tilemap.SetTile((Vector3Int)pos, after);

            if (TryGetTile(pos, out var afterTile))
            {
                if (before.HasParent)
                    afterTile.Parent = before.Parent;

                OnTileChanged?.Invoke(new TileChangedInfo(pos, before, afterTile));
                return afterTile;
            }
            return null;
        }


        public void Messy(Vector2Int tilePos, IMultipleVisual visual)
        {
            ChangeTile(tilePos, TileType.Messy, visual.VisualType);

            TileEffectManager.Singleton.SetEffect(TileEffectType.ToSand, TilePosToWorld(tilePos));
        }
        public void ToSand(Vector2Int tilePos)
        {
            ChangeTile(tilePos, TileType.Sand);
        }
        public void ToEnemyPlant(Vector2Int tilePos)
        {
            ChangeTile(tilePos, TileType.EnemyPlant);
        }


        static public readonly float CELL_SIZE = 0.5f;
    }
}