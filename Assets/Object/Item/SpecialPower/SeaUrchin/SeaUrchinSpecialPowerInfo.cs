using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("SeaUrchin�̃X�y�V�����p���[�p�����[�^")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SeaUrchinSpecialPower")]
    public class SeaUrchinSpecialPowerInfo : ScriptableObject, ISowSeedSpecialPowerInfo
    {
        [SerializeField, Rename("����܂��}�X"), EditTilePos(Direction.NONE, true)]
        ArrayWrapper<Vector2Int> seedLocalTilePos;
        public Vector2Int[] GetSeedLocalTilePos(Vector2Int dir) => seedLocalTilePos.Value;

        [SerializeField, Rename("�Ή��J�n�n�_�܂ł̋���(n�}�X)")]
        int distance = 5;
        public int Distance => distance;

        [SerializeField, Rename("�Ή��J�n�n�_�ɂ����X�s�[�h(n�}�X/�b)")]
        float speed = 10;
        public float Speed => speed;

        [SerializeField, Rename("������߂Â���������[�V�������n�߂�A�Ή��J�n�n�_�܂ł̋���(n�}�X)")]
        float startSowSeedDistance = 1.0f;
        public float StartSowSeedDistance => startSowSeedDistance;

    }
}
