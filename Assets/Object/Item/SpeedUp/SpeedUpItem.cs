using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class SpeedUpItem : ItemBase
    {
        SpeedUpItemInfo Info => ItemBaseInfo as SpeedUpItemInfo;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            PlayerMover.Singleton.SpeedUp(Info.SpeedUp);

            StatusChangeManager.Singleton.AddStatus(new(Info.Duration, Icon));

            IsFinishUse = true;

            yield return new WaitForSeconds(Info.Duration);

            PlayerMover.Singleton.SpeedDown(Info.SpeedUp);
        }
    }
}
