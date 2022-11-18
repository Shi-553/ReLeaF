using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace ReLeaf
{
    public class GreeningRate : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        float value;
        public float Value => value;
        public float ValueRate => value / DungeonManager.Instance.MaxGreeningCount;

        [SerializeField]
        float targetRate = 0.8f;
        [SerializeField]
        Transform targetRateTransform ;

        [SerializeField]
        Slider slider;
        public static GreeningRate Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        private void Start()
        {
            value = 0;
            slider.value = ValueRate;
            var sliderRect = slider.GetComponent<RectTransform>();

            var targetRatePos = targetRateTransform.position;
            targetRatePos.x = Mathf.Lerp(sliderRect.position.x - sliderRect.sizeDelta.x, sliderRect.position.x - sliderRect.sizeDelta.x, targetRate);
            targetRateTransform.position = targetRatePos;
        }

        // —Î‰»
        public void Greening(int val)
        {
            value += val;
            slider.value = ValueRate;
        }
        public void Deserting(int val)
        {
            value -= val;
            slider.value = ValueRate;
        }

    }
}
