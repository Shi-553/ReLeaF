using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

namespace ReLeaf
{
    public class RandomTile : TerrainTile
    {
        [Serializable]
        public class RandomInfo
        {
            public float probability=50.0f;
            public GameObject obj;
        }
        
        public RandomInfo[] randomInfos;
        override public GameObject Obj
        {
            get
            {
                var index = MathExtension.GetRandomIndex(randomInfos.Select(r => r.probability).ToArray());
                return randomInfos[index].obj;
            }
        }

#if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/2D/Tiles/RandomTile")]
        public static void CreateRandomTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Random Tile", "New Random Tile", "Asset", "Save Random Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RandomTile>(), path);
        }
#endif
    }
}
