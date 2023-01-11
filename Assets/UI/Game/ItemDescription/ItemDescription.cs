using TMPro;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ItemDescription : SingletonBase<ItemDescription>
    {
        [SerializeField]
        TextMeshProUGUI textMeshPro;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                ResetItemDescription();
            }
        }

        public void SetItemDescription(string text)
        {
            textMeshPro.text = text;
        }
        public void ResetItemDescription()
        {
            textMeshPro.text = "";
        }
    }
}
