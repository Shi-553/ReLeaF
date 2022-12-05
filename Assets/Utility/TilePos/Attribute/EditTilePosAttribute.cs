using System;
using UnityEngine;

namespace Utility
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EditTilePosAttribute : PropertyAttribute
    {
        readonly public Direction direction;
        public EditTilePosAttribute(Direction dir)
        {
            direction = dir;
        }
    }
}