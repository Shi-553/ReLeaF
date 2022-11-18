using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ReLeaf
{
    public enum TileType
    {
        Hole,
        Foundation,
        Rock,
        Plant,
        Door,
        Wall,
        DoorSwitch,
        Sand,
    };

    public class TerrainTile : Tile
    {
        public TileType tileType;
        public bool canSowGrass;
        [SerializeField]
        GameObject gameobject;

        static public Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            if (Application.isPlaying)
            {
                if (go != null)
                    Destroy(go);

                if (tiles.Remove((Vector2Int)position, out var g))
                {
                    Destroy(g);
                }
                if (gameobject != null)
                {
                    if (tiles.TryGetValue((Vector2Int)position, out var tile) && tile != null)
                    {
                        return true;
                    }
                    var pos = tilemap.GetComponent<Tilemap>().CellToWorld(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;
                    var newTile = Instantiate(gameobject, pos,
                        Quaternion.identity,
                        tilemap.GetComponent<Transform>());


                    tiles[(Vector2Int)position] = newTile;

                }
            }
            return true;
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
#if UNITY_EDITOR
              if (Application.isEditor)
                  tileData.gameObject = gameobject;
#endif
        }
#if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/2D/Tiles/TerrainTile")]
        public static void CreateRoadTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Terrain Tile", "New Terrain Tile", "Asset", "Save Terrain Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), path);
        }
#endif
    }
}