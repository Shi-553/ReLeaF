using Pickle;
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
        None = -1,
        Sand,
        Hole,
        Rock,

        Plant,
        Wall,
        Messy,
        Max
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
        [Pickle]
        [SerializeField]
        protected TileObject currentTileObject;
        public TileObject CurrentTileObject => currentTileObject;

        virtual protected void UpdateTileObject(Vector3Int position, ITilemap tilemap) { }

        static DungeonManager dungeonManager;
        static ComponentPool componentPool;

        static Tilemap tilemap;

        protected PoolArray Pools;

        protected virtual IPool Pool { get; }

        public override bool StartUp(Vector3Int position, ITilemap tm, GameObject go)
        {
            if (Application.isPlaying)
            {
                if (go != null)
                {
                    Debug.LogWarning(go.name);
                    Destroy(go);
                }

                if (tilemap == null)
                    tilemap = tm.GetComponent<Tilemap>();

                if (dungeonManager == null)
                    dungeonManager = FindObjectOfType<DungeonManager>();
                if (componentPool == null)
                    componentPool = FindObjectOfType<ComponentPool>();

                if (dungeonManager.tiles.ContainsKey((Vector2Int)position))
                {
                    return true;
                }

                UpdateTileObject(position, tm);
                if (currentTileObject == null)
                {
                    return true;
                }

                Pools ??= componentPool.SetPoolArray<TileObject>(TileType.Max.ToInt32());

                var p = Pool ?? Pools.SetPool(CurrentTileObject.TileType.ToInt32(), CurrentTileObject);

                var newTile = p.Get<TileObject>(tile => tile.transform.position = tilemap.CellToWorld(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2);


                newTile.TilePos = dungeonManager.WorldToTilePos(newTile.transform.position);


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
                tileData.gameObject = currentTileObject.gameObject;

#if UNITY_EDITOR
            if (!tilemap.GetComponent<Tilemap>().gameObject.CompareTag("EditorOnly") && (currentTileObject != null))
                tileData.sprite = null;
#else
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