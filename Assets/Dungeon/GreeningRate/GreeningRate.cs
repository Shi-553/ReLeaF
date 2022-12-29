using UnityEngine;
using Utility;

namespace ReLeaf
{
    [ClassSummary("緑化率マネージャー")]
    public class GreeningRate : MonoBehaviour
    {
        [SerializeField, Rename("クリアに必要な緑化率")]
        float targetRate = 0.7f;

        [SerializeField, ReadOnly]
        float value;
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                GreeningRateUI.Singleton.Slider.value = ValueRate;

            }
        }
        public float ValueRate => value / targetRate / DungeonManager.Singleton.MaxGreeningCount;


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
            GreeningRateUI.Singleton.Slider.value = ValueRate;

            DungeonManager.Singleton.OnTileChanged += OnTileChanged;
        }

        private void OnTileChanged(DungeonManager.TileChangedInfo obj)
        {
            if (obj.beforeTile.TileType != TileType.Foundation &&
                obj.afterTile.TileType == TileType.Foundation)
                Value++;

            if (obj.beforeTile.TileType == TileType.Foundation &&
                obj.afterTile.TileType != TileType.Foundation)
                Value--;

            if (ValueRate >= 1.0f)
            {
                GameRuleManager.Singleton.Finish(true);
            }
        }
    }
}
