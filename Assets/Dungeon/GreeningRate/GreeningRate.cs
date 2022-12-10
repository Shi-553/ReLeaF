using UnityEngine;
using Utility;
using Slider = UnityEngine.UI.Slider;

namespace ReLeaf
{
    [ClassSummary("緑化率マネージャー")]
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
        public float ValueRate => value / StageManager.Singleton.Current.TargetRate / DungeonManager.Singleton.MaxGreeningCount;

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

            if (ValueRate >= 1.0f)
            {
                GameRuleManager.Singleton.Finish(true);
            }
        }
    }
}
