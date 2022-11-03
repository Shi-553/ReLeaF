using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public interface IRoomEnemy
    {
        bool CanAttackPlayer { get; set; }
    }
}