using System.Collections;
using UnityEngine;
using Utility;
using static ReLeaf.Plant;

namespace ReLeaf
{
    public class Messy : TileObject, IMultipleVisual
    {
        [SerializeField]
        MessyInfo messyInfo;

        public VisualType visualType;
        public int VisualType => visualType.ToInt32();

        protected override void InitImpl()
        {
            base.InitImpl();
            StartCoroutine(WaitCure());
            SEManager.Singleton.Play(messyInfo.ChangeSand, DungeonManager.Singleton.TilePosToWorld(TilePos));
        }
        IEnumerator WaitCure()
        {
            yield return new WaitForSeconds(messyInfo.CureTime);
            DungeonManager.Singleton.ToSand(TilePos);
        }
    }
}
