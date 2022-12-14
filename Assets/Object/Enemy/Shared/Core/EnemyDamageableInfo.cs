using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵({asset.dirname})の被ダメージパラメータ")]
    [CreateAssetMenu(menuName = "Enemy/EnemyDamageableInfo")]
    public class EnemyDamageableInfo : ScriptableObject
    {
        [SerializeField, Rename("最大HP")]
        float hpMax = 10;
        public float HPMax => hpMax;

        [SerializeField, Rename("緑化でダメージを受けるマス")]
        LocalTilePos weakLocalTilePos;
        public LocalTilePos WeakLocalTilePos => weakLocalTilePos;
    }
}
