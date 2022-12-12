using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class LocalTilePos
    {

        [SerializeField, Rename("上を向いたときのローカルポジション"), EditTilePos(Direction.UP)]
        ArrayWrapper<Vector2Int> upList;
        [SerializeField, Rename("下を向いたときのローカルポジション"), EditTilePos(Direction.DOWN)]
        ArrayWrapper<Vector2Int> downList;
        [SerializeField, Rename("左を向いたときのローカルポジション"), EditTilePos(Direction.LEFT)]
        ArrayWrapper<Vector2Int> leftList;
        [SerializeField, Rename("右を向いたときのローカルポジション"), EditTilePos(Direction.RIGHT)]
        ArrayWrapper<Vector2Int> rightList;

        public Vector2Int[] GetLocalTilePosList(Vector2Int dir)
        {
            if (dir.y > 0)
                return upList.Value;
            if (dir.y < 0)
                return downList.Value;
            if (dir.x < 0)
                return leftList.Value;
            if (dir.x > 0)
                return rightList.Value;

            return downList.Value;
        }
    }
}