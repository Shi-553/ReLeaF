using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{

    public class DungeonEditor : EditorWindow
    {
        static Tilemap groundTilemap;
        static Tilemap wallTilemap;
        static TileBase wallTile;
        static bool isReGenerate = true;

        [MenuItem("Window/2D/DungeonCreateEditor")]
        static void Open()
        {
            var window = GetWindow<DungeonEditor>();
            window.titleContent = new GUIContent("DungeonCreateEditor");
        }
        private void OnGUI()
        {

            GUILayout.Label("Generate Wall", EditorStyles.boldLabel);

            groundTilemap = EditorGUILayout.ObjectField("Ground Layer", groundTilemap, typeof(Tilemap), true) as Tilemap;
            wallTilemap = EditorGUILayout.ObjectField("Wall Layer", wallTilemap, typeof(Tilemap), true) as Tilemap;

            wallTile = EditorGUILayout.ObjectField("Wall Tile", wallTile, typeof(TileBase), true) as TileBase;

            isReGenerate = EditorGUILayout.Toggle("Is ReGenerate", isReGenerate);

            if (GUILayout.Button("Generate Wall!!"))
            {
                if (groundTilemap == null || wallTilemap == null || wallTile == null)
                {
                    Debug.LogError("Null Reference Exception");
                    return;
                }
                if (isReGenerate)
                {
                    wallTilemap.ClearAllTiles();
                }
                foreach (var tilePos in groundTilemap.cellBounds.allPositionsWithin)
                {
                    if (groundTilemap.GetTile(tilePos) == null)
                    {
                        continue;
                    }
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            var pos = new Vector3Int(tilePos.x + i, tilePos.y + j, tilePos.z);

                            if (groundTilemap.GetTile(pos) == null &&
                                wallTilemap.GetTile(pos) == null)
                            {
                                wallTilemap.SetTile(pos, wallTile);
                            }
                        }
                    }
                }

            }
        }
    }
}