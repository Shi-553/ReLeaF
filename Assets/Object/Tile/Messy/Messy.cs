using System.Collections;
using UnityEngine;
using Utility;
using static ReLeaf.Plant;

namespace ReLeaf
{
    public class Messy : TileObject, IMultipleVisual
    {
        MessyInfo MessyInfo => Info as MessyInfo;

        public VisualType visualType;
        public int VisualType => visualType.ToInt32();

        public int VisualMax => Plant.VisualType.Max.ToInt32();
        protected override void InitImpl()
        {
            base.InitImpl();
            StartCoroutine(WaitCure());
            SEManager.Singleton.Play(MessyInfo.ChangeSand, DungeonManager.Singleton.TilePosToWorld(TilePos));
        }
        IEnumerator WaitCure()
        {
            yield return new WaitForSeconds(MessyInfo.CureTime);
            DungeonManager.Singleton.ToSand(TilePos);
        }
    }
}
