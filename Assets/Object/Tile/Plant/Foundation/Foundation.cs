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


        public void SetHighlight(bool sw)
        {
            highLightObj.SetActive(sw);
        }


    }
}