using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class EnemyStanItem : ItemBase
    {
        EnemyStanItemInfo Info => ItemBaseInfo as EnemyStanItemInfo;

        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, List<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {

            yield return new WaitForSeconds(Info.Duration);


        }
    }
}
