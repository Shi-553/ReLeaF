using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class ItemSelectorUI : SingletonBase<ItemSelectorUI>
    {
        [SerializeField]
        RectTransform itemsRoot;
        public RectTransform ItemsRoot => itemsRoot;
        [SerializeField]
        RectTransform selector;
        public RectTransform Selector => selector;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
    }
}
