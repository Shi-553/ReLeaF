using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReLeaf
{
    public abstract class TileObject : MonoBehaviour, IPoolableSelfRelease
    {
        public static TileObject NullTile;

        [SerializeField, Rename("タイルタイプ")]
        protected TileType tileType=TileType.None;
        public TileType TileType => tileType;

        public bool CanEnemyMove => TileType == TileType.Plant|| TileType == TileType.Sand;
        public bool CanEnemyAttack(bool isDamagableOnly) => TileType == TileType.Plant || (isDamagableOnly && TileType == TileType.Sand);

        public Vector2Int TilePos { get;  set; }
        private void OnEnable()
        {
        }
        public virtual void Init(bool isCreated)
        {
        }

        public virtual void Uninit()
        { 
        }

        [SerializeField, Rename("種をまけるかどうか")]
        protected bool canSowGrass;
        public bool CanSowGrass => canSowGrass;

        IPool IPoolableSelfRelease.Pool { get; set; }

    }
}
