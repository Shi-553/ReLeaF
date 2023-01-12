using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class DungeonGreeningRateUI : SingletonBase<DungeonGreeningRateUI>
    {
        [SerializeField]
        Slider slider;

        public Slider Slider => slider;

        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
        }
    }
}
