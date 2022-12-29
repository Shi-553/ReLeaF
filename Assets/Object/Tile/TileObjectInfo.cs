using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("タイルオブジェクト({asset.dirname})の情報")]
    [CreateAssetMenu(menuName = "Tile/TileObjectInfo")]
    public class TileObjectInfo : ScriptableObject
    {

        [SerializeField, Rename("タイルタイプ")]
        TileType tileType = TileType.None;
        public TileType TileType => tileType;

        [SerializeField, Rename("敵が上を歩けるか")]
        bool canEnemyMove;
        public bool CanEnemyMove => canEnemyMove;

        [SerializeField, Rename("敵の攻撃を受けるか")]
        bool canEnemyAttack;
        public bool CanEnemyAttack => canEnemyAttack;

        [SerializeField, Rename("敵の移動攻撃を受けるか")]
        bool canEnemyMoveAttack;
        public bool CanEnemyMoveAttack => canEnemyMoveAttack;

        [SerializeField, Rename("緑化できるか")]
        bool canGreening;
        public bool CanGreening => canGreening;

        [SerializeField, Rename("スペシャルで緑化できるか")]
        bool canGreeningUseSpecial;
        public bool CanGreeningUseSpecial => canGreeningUseSpecial;

        [SerializeField, Rename("すでに緑化済みか")]
        bool isAlreadyGreening;
        public bool IsAlreadyGreening => isAlreadyGreening;

    }
}
