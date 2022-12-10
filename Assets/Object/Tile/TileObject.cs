using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class TileObject : PoolableMonoBehaviour
    {
        [SerializeField]
        TileObjectInfo info;
        protected TileObjectInfo Info => info;

        public TileType TileType => info.TileType;
        public bool CanEnemyMove => info.CanEnemyMove;
        public bool CanEnemyAttack(bool includeMoveabePos) => info.CanEnemyAttack || (includeMoveabePos && info.CanEnemyMove);
        public virtual bool CanGreening(bool useSpecial) => useSpecial ? info.CanGreeningUseSpecial : info.CanGreening;
        public virtual bool IsAlreadyGreening => info.IsAlreadyGreening;

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
