﻿using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class TileObject : MonoBehaviour, IPoolableSelfRelease
    {
        public static TileObject NullTile;

        [SerializeField, Rename("タイルタイプ")]
        protected TileType tileType = TileType.None;
        public TileType TileType => tileType;

        public bool CanEnemyMove => TileType == TileType.Plant || TileType == TileType.Sand || TileType == TileType.Messy;
        public bool CanEnemyAttack(bool isDamagableOnly) => TileType == TileType.Plant || (isDamagableOnly && (TileType == TileType.Sand || TileType == TileType.Messy));

        public Vector2Int TilePos { get; set; }
        public bool IsInvincible { get; set; }
        public virtual void Init(bool isCreated)
        {
        }

        public virtual void Uninit()
        {
            IsInvincible = false;
        }

        [SerializeField, Rename("種をまけるかどうか")]
        protected bool canSowGrass;
        public bool CanSowGrass => canSowGrass;

        IPool IPoolableSelfRelease.Pool { get; set; }

    }
}
