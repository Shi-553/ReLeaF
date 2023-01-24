using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public class TileObject : PoolableMonoBehaviour, ISetRoom
    {
        [SerializeField]
        TileObjectInfo info;
        protected TileObjectInfo Info => info;

        // インスタンス化する子供
        public virtual TileObject InstancingTarget => this;

        // このタイルをインスタンス化した親
        public TileObject Parent { get; set; }
        public bool HasParent => Parent != null;
        public TileObject ParentOrThis => HasParent ? Parent : this;

        public TileBase CreatedTile { get; set; }

        public TileType TileType => info.TileType;

        public bool CanEnemyAttack(bool includeMoveabePos) => info.CanEnemyAttack || (includeMoveabePos && info.CanEnemyMove);

        public bool CanEnemyMove(bool isAttackMove) => (isAttackMove ? CanEnemyMoveAttack(true) : info.CanEnemyMove);

        public bool CanEnemyMoveAttack(bool includeMoveabePos) => info.CanEnemyMoveAttack || (includeMoveabePos && info.CanEnemyMove);

        public virtual bool CanGreening(bool useSpecial) => useSpecial ? info.CanGreeningUseSpecial : info.CanGreening;
        public virtual bool IsAlreadyGreening => info.IsAlreadyGreening;

        public bool IsOnlyNormalFoundation => info.IsOnlyNormalFoundation;

        public bool CanOrAleadyGreening(bool useSpecial) => CanGreening(useSpecial) || IsAlreadyGreening;

        public Vector2Int TilePos { get; set; }
        public bool IsInvincible { get; set; }

        new Collider2D collider2D;

        bool enableCollider = true;
        public bool EnableCollider
        {
            get => enableCollider;
            set
            {
                if (enableCollider == value)
                    return;

                enableCollider = value;
                if (collider2D != null)
                    collider2D.enabled = value;
            }
        }
        MeshRenderer[] renderers;

        bool enableRenderer = true;
        public bool EnableRenderer
        {
            get => enableRenderer;
            set
            {
                if (enableRenderer == value)
                    return;

                enableRenderer = value;

                if (renderers == null)
                    renderers = GetComponentsInChildren<MeshRenderer>(true);

                foreach (var renderer in renderers)
                {
                    renderer.enabled = value;
                }
            }
        }

        protected virtual void Start()
        {
            collider2D = GetComponentInChildren<Collider2D>();
            renderers = GetComponentsInChildren<MeshRenderer>(true);
        }
        protected override void InitImpl()
        {
            EnableCollider = true;
            EnableRenderer = true;
        }
        protected override void UninitImpl()
        {
            IsInvincible = false;
        }
        public Room Room { get; private set; }
        public virtual void SetRoom(Room room) => Room = room;
    }
}
