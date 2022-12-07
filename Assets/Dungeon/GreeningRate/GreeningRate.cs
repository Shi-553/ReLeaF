using UnityEngine;
using Utility;
using Slider = UnityEngine.UI.Slider;

namespace ReLeaf
{
    [ClassSummary("�Ή����}�l�[�W���[")]
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

            }
        }
        public float ValueRate => value / DungeonManager.Singleton.MaxGreeningCount;

        [SerializeField]
        Transform targetRateTransform;

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

            var targetRatePos = targetRateTransform.localPosition;
            targetRatePos.x = Mathf.Lerp(-sliderRect.sizeDelta.x / 2, sliderRect.sizeDelta.x / 2, StageManager.Singleton.Current.TargetRate);
            targetRateTransform.localPosition = targetRatePos;

            slider.maxValue = StageManager.Singleton.Current.TargetRate;  //�X���C�_�[�̍ő�l���^�[�Q�b�g�̍ő�l�ɐݒ�

            DungeonManager.Singleton.OnTileChanged += OnTileChanged;
        }

        private void OnTileChanged(DungeonManager.TileChangedInfo obj)
        {
            if (obj.beforeTile.TileType != TileType.Plant &&
                obj.afterTile.TileType == TileType.Plant)
                Value++;

            if (obj.beforeTile.TileType == TileType.Plant &&
                obj.afterTile.TileType != TileType.Plant)
                Value--;

            if (ValueRate >= StageManager.Singleton.Current.TargetRate)
            {
                GameRuleManager.Singleton.Finish(true);
            }
        }
    }
}
