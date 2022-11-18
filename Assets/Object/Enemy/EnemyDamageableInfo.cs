using UnityEngine;

namespace ReLeaf
{
    [CreateAssetMenu(menuName = "Enemy/EnemyDamageableInfo")]
    public class EnemyDamageableInfo : ScriptableObject
    {
        [SerializeField]
        float hpMax=10;
        public float HPMax => hpMax;

        [SerializeField]
        Vector2Int[] weakLocalTilePos;
         public Vector2Int[] WeakLocalTilePos => weakLocalTilePos;
    }
}
