using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public class DungeonEditor : EditorWindow
    {
        static DungeonEditorInfo info;
        static DungeonEditorInfo Info => info = info == null ?
           AssetDatabase.LoadAssetAtPath<DungeonEditorInfo>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + nameof(DungeonEditorInfo)).FirstOrDefault()))
            : info;

        static Tilemap tilemap;
        static Tilemap Tilemap => tilemap = tilemap == null ? FindObjectOfType<Tilemap>() : tilemap;

        static List<Vector3Int> buffer = new();

        [MenuItem("Window/2D/DungeonEditor")]
        static void Open()
        {
            var window = GetWindow<DungeonEditor>();
            window.titleContent = new GUIContent("DungeonEditor");

        }

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
        public Transform OuterWall
        {
            get
            {
                var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();

                var obj = roots.FirstOrDefault(g => g.name == OUTER_WALL_NAME);
                if (obj == null)
                {
                    foreach (var root in roots)
                    {
                        var t = root.transform.Find(OUTER_WALL_NAME);
                        if (t != null)
                            return t;
                    }
                }

                return obj.transform;
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("自動生成した壁を消す", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(Tilemap, "自動生成した壁を消す");

                OuterWall.gameObject.SetActive(false);

                foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = Tilemap.GetTile<TerrainTile>(tilePos);
                    if (tile == Info.AltWallTile)
                    {
                        Tilemap.SetTile(tilePos, null);
                    }
                }
                EditorSceneManager.MarkSceneDirty(Tilemap.gameObject.scene);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("手動で配置した壁を消す", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(Tilemap, "手動で配置した壁を消す");

                foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = Tilemap.GetTile<TerrainTile>(tilePos);
                    if (tile == Info.WallTile)
                    {
                        Tilemap.SetTile(tilePos, null);
                    }
                }
                EditorSceneManager.MarkSceneDirty(Tilemap.gameObject.scene);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("壁を自動生成", GUILayout.Height(30)))
            {
                EditorSceneManager.MarkSceneDirty(Tilemap.gameObject.scene);


                foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = Tilemap.GetTile<TerrainTile>(tilePos);
                    if (tile == Info.AltWallTile)
                    {
                        Tilemap.SetTile(tilePos, null);
                    }
                }

                buffer.Clear();
                foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                {
                    if (!NeedToGenerate(tilePos))
                    {
                        continue;
                    }
                    buffer.Add(tilePos);
                }

                int minX = Tilemap.cellBounds.xMax;
                int maxX = Tilemap.cellBounds.xMin;
                int minY = Tilemap.cellBounds.yMax;
                int maxY = Tilemap.cellBounds.yMin;

                foreach (var tilePos in buffer)
                {
                    if (minX > tilePos.x) minX = tilePos.x;
                    if (maxX < tilePos.x) maxX = tilePos.x;
                    if (minY > tilePos.y) minY = tilePos.y;
                    if (maxY < tilePos.y) maxY = tilePos.y;
                }
                minX--;
                maxX++;
                minY--;
                maxY++;

                var outerWallTrans = OuterWall;
                var maskTrans = outerWallTrans.GetChild(0);

                outerWallTrans.gameObject.SetActive(true);

                Vector3 centerPos = TilePosToWorld(new Vector2(minX + maxX, minY + maxY) / 2);
                Vector3 fixedCenterPos = TilePosToWorld(new Vector2Int(minX + maxX, minY + maxY) / 2);
                fixedCenterPos.x += 0.25f;
                fixedCenterPos.y += 0.25f;

                fixedCenterPos.z = -0.495f;
                centerPos.z = -0.495f;
                outerWallTrans.position = fixedCenterPos;

                maskTrans.localScale = new Vector2(maxX - minX + 1, maxY - minY + 1) / 2;
                maskTrans.position = centerPos;

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        var pos = new Vector3Int(x, y, 0);
                        var tile = Tilemap.GetTile(pos);
                        if (tile == null)
                        {
                            Tilemap.SetTile(pos, Info.AltWallTile);
                        }
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
            if (current == null || current == Info.AltWallTile || current == Info.WallTile)
                return false;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var pos = new Vector3Int(tilePos.x + x, tilePos.y + y, tilePos.z);
                    var tile = Tilemap.GetTile(pos);
                    if (tile == null || tile == Info.AltWallTile || tile == Info.WallTile)
                        return true;
                }
            }
            return false;
        }
    }
}