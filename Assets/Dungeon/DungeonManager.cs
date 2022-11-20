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



        public int MaxGreeningCount { get; private set; }

        public struct TileChangedInfo
        {
            public Vector2Int tilePos;
            public TerrainTile beforeTile;
            public TerrainTile afterTile;

            public TileChangedInfo(Vector2Int tilePos, TerrainTile beforeTile, TerrainTile afterTile)
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
                if (tile != null && tile.canSowGrass)
                {
                    MaxGreeningCount++;
                }
            }
        }
        public Vector2Int WorldToTilePos(Vector3 worldPos)
        {
            return (Vector2Int)grid.WorldToCell(worldPos);
        }
        public Vector3 TilePosToWorld(Vector2Int tilePos)
        {
            return grid.CellToWorld((Vector3Int)tilePos) + new Vector3(CELL_SIZE, CELL_SIZE) / 2;
        }
        public TerrainTile GetGroundTile(Vector2Int pos)
        {
            return groundTilemap.GetTile<TerrainTile>((Vector3Int)pos);
        }

        public void Messy(Plant plant)
        {
            var tilePos = plant.TilePos;
            ChangeTile(tilePos, null, messyTile);
            StartCoroutine(CureMessy(tilePos));
        }

        public bool CanSowSeed(TerrainTile tile, PlantType type)
        {
            if (type < 0 || seedTiles.Length <= (int)type)
            {
                return false;
            }

            if (tile == null)
            {
                return false;
            }
            if (type == PlantType.Foundation && !tile.canSowGrass)
            {
                return false;
            }

            return true;
        }
        public bool CanSowSeed(Vector2Int tilePos, PlantType type)
        {
            var tile = groundTilemap.GetTile<TerrainTile>((Vector3Int)tilePos);
            return CanSowSeed(tile,type);
        }

        public bool SowSeed(Vector2Int tilePos, PlantType type)
        {
            var tile = groundTilemap.GetTile<TerrainTile>((Vector3Int)tilePos);
            if (!CanSowSeed(tile, type))
            {
                return false;
            }

            ChangeTile(tilePos, tile, seedTiles[(int)type]);
            return true;
        }

        void ChangeTile(Vector2Int pos, TerrainTile before, TerrainTile after)
        {
            if (TerrainTile.tiles.Remove(pos, out var go))
            {
                Destroy(go);
            }
            before = before == null ? groundTilemap.GetTile<TerrainTile>((Vector3Int)pos) : before;

            groundTilemap.SetTile((Vector3Int)pos, after);
            OnTileChanged?.Invoke(new TileChangedInfo(pos, before, after));
        }

        IEnumerator CureMessy(Vector2Int tilePos)
        {
            yield return new WaitForSeconds(messyCuredTime);
            ChangeTile(tilePos,messyTile, sandTile);
        }

        public void ToSand(Vector2Int tilePos)
        {
            ChangeTile(tilePos, null, sandTile);
        }

        static public readonly float CELL_SIZE = 0.5f;
    }
}