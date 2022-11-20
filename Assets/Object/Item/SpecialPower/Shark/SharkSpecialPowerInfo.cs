using UnityEngine;

namespace ReLeaf
{
    [ClassSummary("Sharkのスペシャルパワーパラメータ")]
    [CreateAssetMenu(menuName = "Item/SpecialPower/SharkSpecialPower")]
    public class SharkSpecialPowerInfo : ScriptableObject
    {

        [SerializeField,Rename("種をまくマス", "(上向きが基準のローカルポジション)")]
        Vector2Int[] seedLocalTilePos;
        public Vector2Int[] SeedLocalTilePos => seedLocalTilePos;
    }
}
