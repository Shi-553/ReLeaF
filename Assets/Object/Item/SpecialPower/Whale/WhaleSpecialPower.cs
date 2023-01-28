using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class WhaleSpecialPower : ItemBase
    {
        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            var room = PlayerMover.Singleton.Room;

            if (room == null)
            {
                UseCount = 0;
                yield break;
            }

            room.GreeningRoom();

            yield break;
        }
    }
}
