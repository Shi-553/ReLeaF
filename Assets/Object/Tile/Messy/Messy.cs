using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class Messy : TileObject
    {
        [SerializeField]
        MessyInfo messyInfo;
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
