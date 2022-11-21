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

        static DungeonManager dungeonManager;

         Transform tileParent;

        static Tilemap tilemap;

        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
            if (Application.isPlaying)
            {
                if (go != null)
                {
                    Debug.LogWarning(go.name);
                    Destroy(go);
                }
                if (Obj == null)
                {
                    return true;
                }

                if(dungeonManager==null)
                    dungeonManager=tm.GetComponent<Transform>().GetComponentInParent<DungeonManager>();

                if (dungeonManager.tiles.ContainsKey((Vector2Int)position))
                {
                    return true;
                }

                if (tilemap == null)
                    tilemap = tm.GetComponent<Tilemap>();

                if (tileParent == null)
                {
                    tileParent = tm.GetComponent<Transform>();
                }
                var pos = tilemap.CellToWorld(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;

                var newTile = Instantiate(Obj, pos,
                    Quaternion.identity,
                    tileParent);

                dungeonManager.tiles[(Vector2Int)position] = newTile;
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

            if (!Application.isPlaying)
                tileData.gameObject = Obj;

#if UNITY_EDITOR
            if (!tilemap.GetComponent<Tilemap>().gameObject.CompareTag("EditorOnly") && (Obj != null))
                tileData.sprite = null;
#else
            if(Obj != null)
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