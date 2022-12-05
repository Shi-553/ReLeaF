using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class LocalTilePos
    {

        [SerializeField, Rename("����������Ƃ��̃��[�J���|�W�V����"), EditTilePos]
        ArrayWrapper<Vector2Int> upList;
        [SerializeField, Rename("�����������Ƃ��̃��[�J���|�W�V����"), EditTilePos]
        ArrayWrapper<Vector2Int> downList;
        [SerializeField, Rename("�����������Ƃ��̃��[�J���|�W�V����"), EditTilePos]
        ArrayWrapper<Vector2Int> leftList;
        [SerializeField, Rename("�E���������Ƃ��̃��[�J���|�W�V����"), EditTilePos]
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

            return null;
        }
    }
}