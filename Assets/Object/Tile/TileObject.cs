using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace ReLeaf
{
    public class TileObject : PoolableMonoBehaviour
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

        public bool CanOrAleeadyGreening(bool useSpecial) => CanGreening(useSpecial) || IsAlreadyGreening;

        public Vector2Int TilePos { get; set; }
        public bool IsInvincible { get; set; }

        protected override void InitImpl()
        {
        }
        protected override void UninitImpl()
        {
            IsInvincible = false;
        }
    }
}
