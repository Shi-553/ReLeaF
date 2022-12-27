using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class PlayerStatusUI : SingletonBase<PlayerStatusUI>
    {
        public override bool DontDestroyOnLoad => false;

        [SerializeField]
        Slider hpSlider;
        public Slider HPSlider => hpSlider;

        [SerializeField]
        Slider enelgySlider;
        public Slider EnelgySlider => enelgySlider;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {

        }

    }
}
