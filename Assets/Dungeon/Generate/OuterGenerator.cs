using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public class OuterGenerator
    {
        TerrainTile wallTile;
        TerrainTile altWallTile;
        public OuterGenerator(TerrainTile wallTile, TerrainTile altWallTile)
        {
            this.wallTile = wallTile;
            this.altWallTile = altWallTile;
        }

        public Tilemap Tilemap => DungeonManager.Singleton.Tilemap;

        public Vector2 TilePosToWorld(Vector2Int tilePos)
        {
            return (Vector2)Tilemap.CellToWorld((Vector3Int)tilePos) + new Vector2(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;
        }
        public Vector2 TilePosToWorld(Vector2 tilePos)
        {
            var floor = Vector2Int.FloorToInt(tilePos);
            var smallNumber = tilePos - floor;
            return (Vector2)Tilemap.CellToWorld((Vector3Int)floor) + (smallNumber * DungeonManager.CELL_SIZE) + new Vector2(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;
        }

        const string OUTER_WALL_NAME = "OuterWall";
        Transform outerWall;
        public Transform OuterWall => outerWall = (outerWall != null) ? outerWall : Find(OUTER_WALL_NAME);

        const string SPAWN_LAKE_NAME = "SpawnLake";
        Transform spawnLake;


        public Transform SpawnLake => spawnLake = (spawnLake != null) ? spawnLake : Find(SPAWN_LAKE_NAME);

        Transform Find(string name)
        {
            var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            var obj = roots.FirstOrDefault(g => g.name == name);
            if (obj == null)
            {
                foreach (var root in roots)
                {
                    var t = root.transform.Find(name);
                    if (t != null)
                        return t;
                }
            }
            return obj.transform;
        }

        public void RemoveGeneratedWall()
        {
            OuterWall.gameObject.SetActive(false);

            foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
            {
                var tile = Tilemap.GetTile<TerrainTile>(tilePos);
                if (tile == altWallTile)
                {
                    Tilemap.SetTile(tilePos, null);
                }
            }
        }
        public void RemoveWall()
        {
            foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
            {
                var tile = Tilemap.GetTile<TerrainTile>(tilePos);
                if (tile == wallTile)
                {
                    Tilemap.SetTile(tilePos, null);
                }
            }
        }

        public void GenerateWall()
        {
            int minX = Tilemap.cellBounds.xMax;
            int maxX = Tilemap.cellBounds.xMin;
            int minY = Tilemap.cellBounds.yMax;
            int maxY = Tilemap.cellBounds.yMin;

            foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
            {
                if (!NeedToGenerate(tilePos))
                {
                    continue;
                }

                if (minX > tilePos.x) minX = tilePos.x;
                if (maxX < tilePos.x) maxX = tilePos.x;
                if (minY > tilePos.y) minY = tilePos.y;
                if (maxY < tilePos.y) maxY = tilePos.y;
            }
            minX--;
            maxX++;
            minY--;
            maxY++;

            var maskTrans = OuterWall.GetChild(0);

            OuterWall.gameObject.SetActive(true);

            Vector3 centerPos = TilePosToWorld(new Vector2(minX + maxX, minY + maxY) / 2);
            Vector3 fixedCenterPos = TilePosToWorld(new Vector2Int(minX + maxX, minY + maxY) / 2);
            fixedCenterPos.x += 0.25f;
            fixedCenterPos.y += 0.25f;

            fixedCenterPos.z = -0.495f;
            centerPos.z = -0.495f;
            OuterWall.position = fixedCenterPos;

            maskTrans.localScale = new Vector2(maxX - minX + 1, maxY - minY + 1) / 2;
            maskTrans.localScale -= maskTrans.localScale / 100.0f;
            maskTrans.position = centerPos;


            centerPos.z = 0.1f;
            SpawnLake.position = centerPos;
            SpawnLake.localScale = maskTrans.localScale;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = Tilemap.GetTile(pos);
                    if (tile == null)
                    {
                        Tilemap.SetTile(pos, altWallTile);
                    }
                }
            }
        }
        /// <summary>
        /// 生成する必要があるかどうか
        /// </summary>
        bool NeedToGenerate(Vector3Int tilePos)
        {
            var current = Tilemap.GetTile(tilePos);
            if (current == null || current == altWallTile || current == wallTile)
                return false;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var pos = new Vector3Int(tilePos.x + x, tilePos.y + y, tilePos.z);
                    var tile = Tilemap.GetTile(pos);
                    if (tile == null || tile == altWallTile || tile == wallTile)
                        return true;
                }
            }
            return false;
        }
    }
}
