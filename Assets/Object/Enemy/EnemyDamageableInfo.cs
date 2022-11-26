using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵({asset.dirname})の被ダメージパラメータ")]
    [CreateAssetMenu(menuName = "Enemy/EnemyDamageableInfo")]
    public class EnemyDamageableInfo : ScriptableObject
    {
        [SerializeField,Rename("最大HP")]
        float hpMax=10;
        public float HPMax => hpMax;

        [SerializeField, Rename("緑化でダメージを受けるマス", "(上向きが基準のローカルポジション)")]
        Vector2Int[] weakLocalTilePos;
         public Vector2Int[] WeakLocalTilePos => weakLocalTilePos;
    }
}
