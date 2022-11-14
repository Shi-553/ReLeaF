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
        float recoverySpeed = 1;

        [SerializeField,ReadOnly]
        float value;
        public float Value => value;
        public float ValueRate => value / valueMax;

        [SerializeField]
        bool canOverConsumeOnlyOnce=false;

        [SerializeField]
        Slider slider;
        private void Awake()
        {
            value = valueMax;
            slider.value = ValueRate;
        }

        // ぽいんと消費
        public bool ConsumeValue(float consume)
        {
            if(value==0)
                return false;

            if (value - consume < 0)
            {
                value = 0;
                return canOverConsumeOnlyOnce;
            }
            value -= consume;

            return true;
        }

        private void Update()
        {
            value += recoverySpeed * Time.deltaTime;
            if (value > valueMax)
            {
                value = valueMax;
            }
            slider.value = ValueRate;
        }

    }
}
