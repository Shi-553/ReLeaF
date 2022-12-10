using DebugLogExtension;
using Pickle;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public enum TileType
    {
        None = -1,
        Sand,
        Hole,
        Rock,

        Foundation,
        Wall,
        Messy,
        EnemyPlant,
        SpwanLake,
        SpwanTarget,
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
        private TileFlags m_Flags = TileFlags.LockAll;

        [SerializeField]
        private Tile.ColliderType m_ColliderType = UnityEngine.Tilemaps.Tile.ColliderType.None;

        [Header("Custom")]
        [Pickle]
        [SerializeField]
        protected TileObject currentTileObject;
        public TileObject CurrentTileObject => currentTileObject;

        virtual protected void UpdateTileObject(Vector3Int position, ITilemap tilemap) { }

        Tilemap tilemap;

        protected PoolArray Pools;

        IPool pool;
        protected virtual IPool Pool => pool;

        public bool IsInvincible { get; set; }

        public int defaultCapacity = 10;
        public int maxSize = 100;
        public bool dontUseTileManager = false;
        public bool isOverrideTile = false;

        protected TileObject createdObject;

        bool isInit = false;
        void OnEnable()
        {
            tilemap = null;
            isInit = false;
        }
        public void Init()
        {
            if (!isInit)
                InitImpl();
        }
        protected virtual void InitImpl()
        {
            isInit = true;
            Pools = PoolManager.Singleton.SetPoolArray<TileObject>(TileType.Max.ToInt32());
        }


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
                {
                    tilemap = tm.GetComponent<Tilemap>();
                    Init();
                    UpdateTileObject(position, tm);
                    pool = Pool ?? Pools.SetPool(CurrentTileObject.TileType.ToInt32(), CurrentTileObject, defaultCapacity, maxSize);
                }

                if (!dontUseTileManager && !isOverrideTile && DungeonManager.Singleton.tiles.ContainsKey((Vector2Int)position))
                {
                    $"{position} is Aleady".DebugLog();
                    return false;
                }

                UpdateTileObject(position, tm);
                if (currentTileObject == null)
                {
                    $"currentTileObject == null".DebugLog();
                    return false;
                }

                using var _ = Pool.Get<TileObject>(out createdObject);

                createdObject.IsInvincible = IsInvincible;
                createdObject.transform.parent = tm.GetComponent<Transform>();
                createdObject.transform.localPosition = tm.GetComponent<Tilemap>().CellToLocal(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;
                createdObject.TilePos = DungeonManager.Singleton.WorldToTilePos(createdObject.transform.position);

                if (isOverrideTile)
                {
                    if (DungeonManager.Singleton.tiles.Remove((Vector2Int)position, out var removed))
                        removed.Release();
                }

                if (!dontUseTileManager)
                    DungeonManager.Singleton.tiles[(Vector2Int)position] = createdObject;
                return true;
            }
            return false;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = m_Sprite;
            tileData.color = m_Color;
            tileData.transform = m_Transform;
            tileData.flags = m_Flags;
            tileData.colliderType = m_ColliderType;
            if (!Application.isPlaying)
            {
                tileData.gameObject = currentTileObject.gameObject;
            }
#if UNITY_EDITOR
            if (!tilemap.GetComponent<Tilemap>().gameObject.CompareTag("EditorOnly") && (currentTileObject != null))
                tileData.sprite = null;
#else
                tileData.sprite = null;
#endif
        }

#if UNITY_EDITOR

        [MenuItem("Assets/Create/Tile/TerrainTile")]
        public static void CreateTerrainTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Terrain Tile", "New Terrain Tile", "Asset", "Save Terrain Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), path);
        }
#endif
    }
}