using DebugLogExtension;
using Pickle;
using System;
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
        ConnectedSeed,
        StageObject,
        Entrance,
        Max
    };

    public enum TileLayerType
    {
        Ground,
        Wall,
    }
    public interface ILayerFixedTile
    {
        public TileLayerType TileLayerType { get; }
    }
    public class TerrainTile : TileBase, ILayerFixedTile
    {

        [SerializeField]
        public Sprite m_Sprite;

        [SerializeField]
        protected Color m_Color = Color.white;



        [SerializeField]
        protected TileFlags m_Flags = TileFlags.LockAll;

        [SerializeField]
        protected Tile.ColliderType m_ColliderType = UnityEngine.Tilemaps.Tile.ColliderType.None;

        [Header("Custom")]
        [Pickle]
        [SerializeField]
        protected TileObject currentTileObject;
        public TileObject CurrentTileObject => currentTileObject;

        protected virtual bool UpdateTileObject(Vector3Int position, ITilemap tilemap, TileObject oldTileObject) => oldTileObject == null;

        [NonSerialized]
        public Tilemap tilemap;

        protected PoolArray Pools;

        Pool pool;
        protected virtual Pool Pool => pool;

        public bool IsInvincible { get; set; }

        public int defaultCapacity = 10;
        public int maxSize = 100;

        [NonSerialized]
        public TileObject createdObject;

        [SerializeField, Rename("塗るレイヤー名")]
        TileLayerType tileLayerType;

        public TileLayerType TileLayerType => tileLayerType;

        bool isInit = false;
        public void OnEnable()
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
            createdObject = null;

            if (Application.isPlaying)
            {
                if (go != null)
                {
                    Debug.LogWarning(go.name);
                    Destroy(go);
                }
                if (!isInit || tilemap == null)
                {
                    if (tilemap == null)
                    {
                        tilemap = tm.GetComponent<Tilemap>();

                    }
                    Init();
                    pool = Pool ?? Pools.SetPool(CurrentTileObject.TileType.ToInt32(), CurrentTileObject, defaultCapacity, maxSize);
                }

                var oldTileObject = DungeonManager.Singleton.GetTile((Vector2Int)position);

                if (!UpdateTileObject(position, tm, oldTileObject))
                {
                    return false;
                }

                if (oldTileObject != null)
                    oldTileObject.Release();

                if (currentTileObject == null)
                {
                    $"currentTileObject == null".DebugLog();
                    return false;
                }

                using (Pool.Get(out createdObject))
                {
                    createdObject.transform.localPosition = tilemap.CellToLocal(position) + new Vector3(DungeonManager.CELL_SIZE, DungeonManager.CELL_SIZE) / 2;

                    InitCreatedObject((Vector2Int)position);

                }

                DungeonManager.Singleton.SetNewTile((Vector2Int)position, createdObject.InstancingTarget);

                return true;
            }
            else
            {
                if (go != null)
                {
                    go.transform.position += new Vector3(0, 0, 0.001f);
                    if (go.TryGetComponent<TileObject>(out var tileObject))
                    {
                        createdObject = tileObject;
                        tileObject.FasterInit();
                        InitCreatedObject((Vector2Int)position);

                    }
                }
            }
            return false;
        }
        void InitCreatedObject(Vector2Int position)
        {
            createdObject.CreatedTile = this;
            createdObject.IsInvincible = IsInvincible;
            createdObject.TilePos = position;
            createdObject.Parent = null;

            if (createdObject != createdObject.InstancingTarget)
            {
                createdObject.InstancingTarget.FasterInit();
                createdObject.InstancingTarget.Parent = createdObject;
                createdObject.InstancingTarget.gameObject.SetActive(true);
                createdObject.InstancingTarget.CreatedTile = this;
                createdObject.InstancingTarget.IsInvincible = createdObject.IsInvincible;
                createdObject.InstancingTarget.TilePos = createdObject.TilePos;
                createdObject.InstancingTarget.Init();
            }

        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = null;
            tileData.color = m_Color;
            tileData.flags = m_Flags;
            tileData.colliderType = m_ColliderType;
#if UNITY_EDITOR

            tileData.sprite = m_Sprite;
            if (!Application.isPlaying && currentTileObject != null)
            {
                tileData.gameObject = currentTileObject.gameObject;
            }
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