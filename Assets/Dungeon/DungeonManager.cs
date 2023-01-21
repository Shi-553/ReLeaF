using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    }
    [ClassSummary("タイルマップマネージャー")]
    public class DungeonManager : SingletonBase<DungeonManager>
    {
        public override bool DontDestroyOnLoad => false;


        [SerializeField]
        Tilemap tilemap;
        public Tilemap Tilemap => tilemap = (tilemap != null) ? tilemap : FindObjectOfType<Tilemap>();

        Dictionary<Vector2Int, TileObject> tileDic = new();

        public ReadOnlyDictionary<Vector2Int, TileObject> TileDic;

#if UNITY_EDITOR
        [Serializable]
        struct SelealizeTileCount
        {
            [ReadOnly]
            public TileType type;
            [ReadOnly]
            public int tileCount;

            public SelealizeTileCount(TileType type, int tileCount)
            {
                this.type = type;
                this.tileCount = tileCount;
            }
        }
        [SerializeField]
        List<SelealizeTileCount> selealizeTileCounts = new();
        Dictionary<TileType, int> tileCounts = new();
#endif
        public void SetNewTile(Vector2Int pos, TileObject newTile)
        {
            tileDic[pos] = newTile;
        }

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
            if (isFirstInit)
            {
                TileDic = new(tileDic);
            }
            if (callByAwake)
            {
                tilemap.RefreshAllTiles();
#if UNITY_EDITOR
                for (int i = 0; i < (int)TileType.Max; i++)
                {
                    tileCounts.Add((TileType)i, 0);
                }
                foreach (var tile in tileDic.Values)
                {
                    tileCounts[tile.TileType]++;
                }
                foreach (var pair in tileCounts)
                {
                    if (pair.Value == 0)
                        continue;
                    selealizeTileCounts.Add(new(pair.Key, pair.Value));
                }

#endif
            }
        }
        protected override void UninitBeforeSceneUnload(bool isDestroy)
        {
            CanChangeTile = false;
            foreach (var tile in tileDic.Values)
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
        public bool TryGetTile(Vector2Int pos, out TileObject tile) => tileDic.TryGetValue(pos, out tile);
        public TileObject GetTile(Vector2Int pos) => tileDic.GetValueOrDefault(pos, null);


        public bool TryGetTile<T>(Vector2Int pos, out T tile) where T : TileObject
        {
            if (tileDic.GetValueOrDefault(pos) is T t)
            {
                tile = t;
                return true;
            }
            tile = null;
            return false;
        }
        public TileObject GetTile<T>(Vector2Int pos) where T : TileObject => tileDic.GetValueOrDefault(pos, null) as T;


        public bool SowSeed(Vector2Int tilePos, bool isSpecial = false, bool isInvincible = false, bool canAleadyGreening = true)
        {
            if (!TryGetTile(tilePos, out var tile))
            {
                return false;
            }

            if (canAleadyGreening && tile.CanOrAleadyGreening(isSpecial))
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
            if (!CanChangeTile || !tileDic.TryGetValue(pos, out var before) || before.TileType == type)
            {
                return null;
            }

            tileDic.Remove(pos);
            before.Release();

            var after = DungeonTilePalette.Singleton.TerrainTileDic[type][index];
            tilemap.SetTile((Vector3Int)pos, after);

            if (TryGetTile(pos, out var afterTile))
            {
                afterTile.SetRoom(before.Room);

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