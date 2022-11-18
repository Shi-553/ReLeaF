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
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                slider.value = ValueRate;

                if (ValueRate >= targetRate)
                {
                    gameclearText.gameObject.SetActive(true);
                }
            }
        }
        public float ValueRate => value / DungeonManager.Instance.MaxGreeningCount;

        [SerializeField]
        float targetRate = 0.8f;
        [SerializeField]
        Transform targetRateTransform ;

        [SerializeField]
        Slider slider;
        [SerializeField]
        GameObject gameclearText;
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

            var targetRatePos = targetRateTransform.localPosition;
            targetRatePos.x = Mathf.Lerp(sliderRect.position.x - sliderRect.sizeDelta.x, sliderRect.position.x - sliderRect.sizeDelta.x, targetRate);
            targetRateTransform.localPosition = targetRatePos;

            DungeonManager.Instance.OnTileChanged += OnTileChanged;
        }

        private void OnTileChanged(DungeonManager.TileChangedInfo obj)
        {
            if (obj.beforeTile.tileType != TileType.Foundation&&
                obj.afterTile.tileType == TileType.Foundation)
                Value++;

            if (obj.beforeTile.tileType != TileType.Sand &&
                obj.afterTile.tileType == TileType.Sand)
                Value--;
        }
    }
}
