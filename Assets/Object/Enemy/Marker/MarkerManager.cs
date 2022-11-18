using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class MarkerManager<T> : MonoBehaviour where T : MarkerBase
    {

        [SerializeField]
        T markerPrefab;
        protected Dictionary<Vector2Int, T> markers = new Dictionary<Vector2Int, T>();

        bool subscribeOnTileChanged;
        protected void Init(bool subscribeOnTileChanged)
        {
            this.subscribeOnTileChanged = subscribeOnTileChanged;
            if (subscribeOnTileChanged)
                DungeonManager.Instance.OnTileChanged += OnTileChanged;

        }
        protected void Uninit()
        {
            if (subscribeOnTileChanged)
                DungeonManager.Instance.OnTileChanged -= OnTileChanged;
        }


        protected T SetMarker(Vector2Int worldTilePos, Transform parent)
        {
            var worldPos = DungeonManager.Instance.TilePosToWorld(worldTilePos);
            var marker = Instantiate(markerPrefab, worldPos, Quaternion.identity, parent);
            markers.Add(worldTilePos, marker);
            marker.Init(worldTilePos);
            return marker;
        }
        protected void ResetAllMarker()
        {
            foreach (var weakMarker in markers)
            {
                if (weakMarker.Value != null)
                    weakMarker.Value.Uninit();
            }
            markers.Clear();
        }
        private void OnTileChanged(DungeonManager.TileChangedInfo info)
        {
            foreach (var key in markers.ToArray())
            {
                key.Value.TileChanged(info);
            }
        }
    }
}
