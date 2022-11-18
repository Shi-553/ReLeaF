using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class ValueGaugeManager : MonoBehaviour
    {
        [SerializeField]
        float valueMax = 10;
        [SerializeField]
        float recoveryBaseSpeed = 1;

        [SerializeField,ReadOnly]
        float value;
        public float Value { get => value;
            set
            {
                this.value= value;
                slider.value = ValueRate;
            }
        }
        public float ValueRate => value / valueMax;

        [SerializeField]
        bool canOverConsumeOnlyOnce=false;

        [SerializeField]
        Slider slider;
        private void Awake()
        {
            Value = valueMax;
        }

        // ぽいんと消費
        public bool ConsumeValue(float consume)
        {
            if(Value == 0)
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
            recovery *= recoveryBaseSpeed;

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
