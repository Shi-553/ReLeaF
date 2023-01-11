using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SpeedUpItem : ItemBase
    {
        [SerializeField]
        SpeedUpItemInfo info;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            PlayerMover.Singleton.SpeedUp(info.SpeedUp);

            StatusChangeManager.Singleton.AddStatus(new(info.Duration, Icon));

            yield return new WaitForSeconds(info.Duration);

            PlayerMover.Singleton.SpeedDown(info.SpeedUp);
        }
    }
}
