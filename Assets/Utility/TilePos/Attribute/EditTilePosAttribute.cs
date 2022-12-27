using System;
using UnityEngine;

namespace Utility
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EditTilePosAttribute : PropertyAttribute
    {
        readonly public Direction direction;
        readonly public bool canSelectMyselfTilePos;

        public EditTilePosAttribute(Direction dir, bool canSelectMyselfTilePos = false)
        {
            direction = dir;
            this.canSelectMyselfTilePos = canSelectMyselfTilePos;
        }
    }
}