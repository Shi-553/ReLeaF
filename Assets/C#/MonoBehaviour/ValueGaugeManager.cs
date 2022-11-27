using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    [ClassSummary("{gameObject.name}ゲージの管理")]
    public class ValueGaugeManager : MonoBehaviour
    {
        [SerializeField, Rename("{gameObject.name}の最大値")]
        float valueMax = 10;

        [SerializeField, ReadOnly]
        float value;
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                slider.value = ValueRate;
            }
        }
        public float ValueRate => value / valueMax;

        [SerializeField, Rename("0をちょうど下回ったフレームにtrueを返すか")]
        bool canOverConsumeOnlyOnce = false;

        [SerializeField]
        Slider slider;
        private void Awake()
        {
            Value = valueMax;
        }

        // ぽいんと消費
        public bool ConsumeValue(float consume)
        {
            if (Value == 0)
                return false;

            if (Value - consume < 0)
            {
                Value = 0;
                return canOverConsumeOnlyOnce;
            }
            Value -= consume;

            return true;
        }
        public bool RecoveryValue(float recovery)
        {

            if (Value == valueMax)
                return false;

            if (Value + recovery > valueMax)
            {
                Value = valueMax;
                return true;
            }
            Value += recovery;

            return true;
        }
    }
}
