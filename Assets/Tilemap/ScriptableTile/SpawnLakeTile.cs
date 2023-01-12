using UnityEditor;
using UnityEngine;

namespace ReLeaf
{
    public class SpawnLakeTile : TerrainTile
    {

#if UNITY_EDITOR

        [MenuItem("Assets/Create/Tile/SpawnLakeTile")]
        public static void CreateSpawnLakeTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save SpawnLake Tile", "New SpawnLake Tile", "Asset", "Save SpawnLake Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnLakeTile>(), path);
        }
#endif
    }
}
