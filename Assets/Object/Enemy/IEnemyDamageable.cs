using UnityEngine;

namespace ReLeaf
{
    public interface IEnemyDamageable
    {
        public void DamagedGreening(Vector2Int tilePos, float atk);
        public void Damaged(float atk);

    }
}
