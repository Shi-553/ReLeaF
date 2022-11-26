using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReLeaf.Plant;
using Utility;

namespace ReLeaf
{
    public class Messy : TileObject, IMultipleVisual
    {
        [SerializeField]
        MessyInfo messyInfo;

        public VisualType visualType;
        public int VisualType => visualType.ToInt32();

        public override void Init(bool isCreated)
        {
            base.Init(isCreated);
            StartCoroutine(WaitCure());
        }
        IEnumerator WaitCure()
        {
            yield return new WaitForSeconds(messyInfo.CureTime);
            DungeonManager.Instance.ToSand(TilePos);
        }
    }
}
