using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class HPRecoverItem : ItemBase
    {
        public override void PreviewRange(Vector2Int tilePos, Vector2Int dir, HashSet<Vector2Int> returns)
        {
        }

        protected override IEnumerator UseImpl(Vector2Int tilePos, Vector2Int dir)
        {
            PlayerCore.Singleton.HPRecoverAll();
            yield break;
        }
    }
}
