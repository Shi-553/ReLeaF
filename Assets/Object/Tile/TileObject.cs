using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class TileObject : PoolableMonoBehaviour
    {
        public static TileObject NullTile;

        [SerializeField, Rename("タイルタイプ")]
        protected TileType tileType = TileType.None;
        public TileType TileType => tileType;

        public bool CanEnemyMove => TileType == TileType.Plant || TileType == TileType.Sand || TileType == TileType.Messy;
        public bool CanEnemyAttack(bool isDamagableOnly) => TileType == TileType.Plant || (!isDamagableOnly && (TileType == TileType.Sand || TileType == TileType.Messy));

        public Vector2Int TilePos { get; set; }
        public bool IsInvincible { get; set; }

        protected override void InitImpl()
        {
        }
        protected override void UninitImpl()
        {
            IsInvincible = false;
        }

        public bool CanSowGrass(bool isSpecial) => TileType == TileType.Sand || (isSpecial && TileType == TileType.Messy);


    }
}
