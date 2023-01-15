using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class AddGreeningRangeItem : ItemBase
    {
        AddGreeningRangeItemInfo Info => ItemBaseInfo as AddGreeningRangeItemInfo;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            PlayerMover.Singleton.AddGreeningRange(Info.AddRange);
            IsFinishUse = true;
            StatusChangeManager.Singleton.AddStatus(new(Info.Duration, Icon));

            yield return new WaitForSeconds(Info.Duration);

            PlayerMover.Singleton.RemoveGreeningRange(Info.AddRange);
        }
    }
}
