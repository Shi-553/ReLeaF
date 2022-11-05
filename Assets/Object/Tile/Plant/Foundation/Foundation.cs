using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class Foundation : Plant
    {
        [SerializeField]
        GameObject highLightObj;

        public bool IsHighlighting => highLightObj.activeSelf;

        private void Start()
        {
            Init();
        }

        protected override void FullGrowed()
        {
            plantObjRoot.SetActive(true);
        }

        public void SetHighlight(bool sw)
        {
            highLightObj.SetActive(sw);
        }


    }
}