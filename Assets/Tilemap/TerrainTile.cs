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

    public class TerrainTile : TileBase
    {

        [SerializeField]
        private Sprite m_Sprite;

        [SerializeField]
        private Color m_Color = Color.white;

        private Matrix4x4 m_Transform = Matrix4x4.identity;


        [SerializeField]
        private TileFlags m_Flags = TileFlags.LockColor;

        [SerializeField]
        private Tile.ColliderType m_ColliderType = Tile.ColliderType.Sprite;

        [Header("Custom")]
        public TileType tileType;
        public bool canSowGrass;
        public GameObject gameobject;
        virtual public GameObject Obj => gameobject;

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
                if (Obj != null)
                {
                    if (tiles.TryGetValue((Vector2Int)position, out var tile) && tile != null)
                    {
                        return true;
                    }
                    var pos = tilemap.GetComponent<Tilemap>().CellToWorld(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;
                    var newTile = Instantiate(Obj, pos,
                        Quaternion.identity,
                        tilemap.GetComponent<Transform>());


                    tiles[(Vector2Int)position] = newTile;

                }
            }
            return true;
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = m_Sprite;
            tileData.color = m_Color;
            tileData.transform = m_Transform;
            tileData.flags = m_Flags;
            tileData.colliderType = m_ColliderType;

            tileData.gameObject = Obj;

#if UNITY_EDITOR
            if (!tilemap.GetComponent<Tilemap>().gameObject.CompareTag("EditorOnly") && Obj != null)
                tileData.sprite = null;
#else
            if(Obj!=null)
                tileData.sprite = null;
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