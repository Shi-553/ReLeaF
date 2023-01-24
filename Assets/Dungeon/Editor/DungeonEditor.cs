using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ReLeaf
{
    public class DungeonEditor : EditorWindow
    {
        static DungeonEditorInfo info;
        static DungeonEditorInfo Info => info = info == null ?
           AssetDatabase.LoadAssetAtPath<DungeonEditorInfo>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + nameof(DungeonEditorInfo)).FirstOrDefault()))
            : info;

        static OuterGenerator generator;

        [MenuItem("Window/2D/DungeonEditor")]
        static void Open()
        {
            var window = GetWindow<DungeonEditor>();
            window.titleContent = new GUIContent("DungeonEditor");

        }
        private void OnEnable()
        {
            generator = new(Info.WallTile, Info.AltWallTile);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("自動生成した壁を消す", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(generator.Tilemap, "自動生成した壁を消す");
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(generator.Tilemap.gameObject.scene);

                generator.RemoveGeneratedWall();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("手動で配置した壁を消す", GUILayout.Height(30)))
            {
                Undo.RegisterCompleteObjectUndo(generator.Tilemap, "手動で配置した壁を消す");
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(generator.Tilemap.gameObject.scene);

                generator.RemoveWall();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("壁を自動生成＆湖を自動調整", GUILayout.Height(30)))
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(generator.Tilemap.gameObject.scene);

                generator.RemoveGeneratedWall();
                generator.GenerateWall();
            }
        }

    }
}