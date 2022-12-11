#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ReLeaf
{
    public class SpawnTargetTile : SandPaddingTile
    {
        SpawnTarget SpawnTarget => currentTileObject as SpawnTarget;
        protected override IMultipleVisual MultipleVisual => SpawnTarget.EnemyInfo;

        public override Vector2Int Size => SpawnTarget.EnemyInfo.EnemyPrefab.TileSize;


#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tile/SpawnTargetTile")]
        public static void CreateSpawnTargetTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save SpawnTargetTile", "New SpawnTargetTile", "Asset", "Save SpawnTargetTile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnTargetTile>(), path);
        }
#endif
    }
}
