using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public class SpawnLakeTile : TerrainTile
    {

        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
            var result = base.StartUp(position, tm, go);
            if (result)
            {
                SpawnLakeManager.Singleton.AddEnabledLake(createdObject as SpawnLake);
            }
            return result;
        }


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
