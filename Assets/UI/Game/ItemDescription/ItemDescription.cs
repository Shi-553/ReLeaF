using TMPro;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ItemDescription : SingletonBase<ItemDescription>
    {
        [SerializeField]
        TextMeshProUGUI textMeshPro;
        [SerializeField]
        RectTransform backGround;

        Transform pivot;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                pivot = transform.GetChild(0);
                ResetItemDescription();
            }
        }

        public void SetItemDescription(string text, Vector3 offset)
        {
            gameObject.SetActive(true);
            pivot.localPosition = offset;
            textMeshPro.text = text;
            textMeshPro.ForceMeshUpdate();

            var backGroundSize = backGround.sizeDelta;
            backGroundSize.x = 20 + textMeshPro.bounds.size.x;
            backGround.sizeDelta = backGroundSize;
        }
        public void ResetItemDescription()
        {
            textMeshPro.text = "";
            gameObject.SetActive(false);
        }
    }
}
