using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("敵({asset.dirname})の移動パラメータ")]
    [CreateAssetMenu(menuName = "Enemy/EnemyMoverInfo")]
    public class EnemyMoverInfo : ScriptableObject
    {
        [SerializeField, Rename("移動スピード(nマス/秒)")]
        float speed = 4;
        public float Speed => speed;

        [SerializeField, Rename("敵の大きさ(nマス)")]
        Vector2Int tileSize = new Vector2Int(2, 2);
        public Vector2Int TileSize => tileSize;

    }
}
