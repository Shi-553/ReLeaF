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

        static bool isReGenerate = false;

        [MenuItem("Window/2D/DungeonCreateEditor")]
        static void Open()
        {
            var window = GetWindow<DungeonEditor>();
            window.titleContent = new GUIContent("DungeonCreateEditor");

        }

        private void OnGUI()
        {
            GUILayout.Label("Generate Wall", EditorStyles.boldLabel);

            isReGenerate = EditorGUILayout.Toggle("Is ReGenerate", isReGenerate);

            GUILayout.Space(10);

            if (GUILayout.Button("Remove Auto Generated Wall", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(Tilemap, "Remove Auto Generated Wall");

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
            if (GUILayout.Button("Remove Normal Wall", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(Tilemap, "Remove Normal Wall");

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
            if (GUILayout.Button("Generate Wall!!", GUILayout.Height(30)))
            {
                EditorSceneManager.MarkSceneDirty(Tilemap.gameObject.scene);

                if (isReGenerate)
                {
                    foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                    {
                        var tile = Tilemap.GetTile(tilePos);
                        if (tile == Info.AltWallTile)
                        {
                            Tilemap.SetTile(tilePos, null);
                        }
                    }
                }
                foreach (var tilePos in Tilemap.cellBounds.allPositionsWithin)
                {
                    if (!NeedToGenerate(tilePos))
                    {
                        continue;
                    }

                    for (int x = -Info.Left; x <= Info.Right; x++)
                    {
                        for (int y = -Info.Down; y <= Info.Up; y++)
                        {
                            var pos = new Vector3Int(tilePos.x + x, tilePos.y + y, tilePos.z);
                            var tile = Tilemap.GetTile(pos);
                            if (tile == null)
                            {
                                Tilemap.SetTile(pos, Info.AltWallTile);
                            }
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