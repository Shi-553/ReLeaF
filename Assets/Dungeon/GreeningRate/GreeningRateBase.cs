using UnityEngine;
using Utility;

namespace ReLeaf
{
    public abstract class GreeningRateBase : MonoBehaviour
    {
        [field: SerializeField, ReadOnly]
        public int MaxGreeningCount { get; protected set; }

        [SerializeField, Rename("目標緑化率")]
        float targetRate = 0.7f;

        [SerializeField, ReadOnly]
        float value;
        public float Value
        {
            get => value;
            protected set
            {
                this.value = value;
                UpdateValue();
            }
        }
        public float ValueRate => value / targetRate / MaxGreeningCount;


        protected abstract void CalculateMaxGreeningCount();
        protected abstract void UpdateValue();
        protected abstract void Finish();

        protected virtual bool IsValidChange(DungeonManager.TileChangedInfo obj) => true;

        protected virtual void Start()
        {
            value = 0;
            UpdateValue();

            DungeonManager.Singleton.OnTileChanged += OnTileChanged;

            CalculateMaxGreeningCount();
        }

        private void OnTileChanged(DungeonManager.TileChangedInfo obj)
        {
            if (!IsValidChange(obj))
                return;

            if (obj.beforeTile.TileType != TileType.Foundation &&
                obj.afterTile.TileType == TileType.Foundation)
                Value++;

            if (obj.beforeTile.TileType == TileType.Foundation &&
                obj.afterTile.TileType != TileType.Foundation)
                Value--;

            if (ValueRate >= 1.0f)
            {
                DungeonManager.Singleton.OnTileChanged -= OnTileChanged;
                Finish();
            }
        }
    }
}
