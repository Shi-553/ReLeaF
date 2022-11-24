using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public enum PlantType
    {
        None = -1,
        Foundation,
        Max
    };
    [ClassSummary("タイルマップマネージャー")]
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField]
        Tilemap groundTilemap;

        [SerializeField]
        TerrainTile sandTile;
        [SerializeField]
        TerrainTile[] seedTiles;

        [SerializeField]
        SelectTile messyTile;

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

            foreach (var pos in groundTilemap.cellBounds.allPositionsWithin)
            {
                var tile = groundTilemap.GetTile<TerrainTile>(pos);
                if (tile != null && tile.CurrentTileObject.CanSowGrass)
                {
                    MaxGreeningCount++;
                }
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
        public TileObject GetTile<T>(Vector2Int pos)where T: TileObject => tiles.GetValueOrDefault(pos, null) as T;

        public bool CanSowSeed(TileObject tile, PlantType type)
        {
            if (type < 0 || seedTiles.Length <= (int)type)
            {
                return false;
            }

            if (tile == null)
            {
                return false;
            }
            if (type == PlantType.Foundation && !tile.CanSowGrass)
            {
                return false;
            }

            return true;
        }
        public bool CanSowSeed(Vector2Int tilePos, PlantType type)
        {
            if (TryGetTile(tilePos, out var tile))
                return CanSowSeed(tile, type);
            return false;
        }

        public bool SowSeed(Vector2Int tilePos, PlantType type)
        {
            if (!CanSowSeed(tilePos, type))
            {
                return false;
            }

            ChangeTile(tilePos, seedTiles[(int)type]);
            return true;
        }

        void ChangeTile(Vector2Int pos, TerrainTile after)
        {

            if (tiles.Remove(pos, out var before))
                before.StaticCast<IPoolableSelfRelease>().Release();
            else
            {
                before = TileObject.NullTile;
            }
            OnTileChanged?.Invoke(new TileChangedInfo(pos, before, after.CurrentTileObject));
            groundTilemap.SetTile((Vector3Int)pos, after);
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

        static public readonly float CELL_SIZE = 0.5f;
    }
}