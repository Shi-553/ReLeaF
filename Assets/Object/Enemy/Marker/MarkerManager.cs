using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class MarkerManager : MonoBehaviour
    {

        Dictionary<Vector2Int, MarkerBase> markers = new();

        public ReadOnlyDictionary<Vector2Int, MarkerBase> Markers;

        [SerializeField]
        bool subscribeOnTileChanged;

        Pool pool;
        [SerializeField]
        MarkerBase marker;

        private void Awake()
        {
            Markers = new ReadOnlyDictionary<Vector2Int, MarkerBase>(markers);
            pool = ComponentPool.Instance.GetPool(marker);
        }


        private void Start()
        {
            if (subscribeOnTileChanged)
                DungeonManager.Instance.OnTileChanged += OnTileChanged;
        }
        
        private void OnDestroy()
        {
            if (subscribeOnTileChanged)
                DungeonManager.Instance.OnTileChanged -= OnTileChanged;
            ResetAllMarker();
        }


        public MarkerBase SetMarker(Vector2Int worldTilePos) => SetMarker<MarkerBase>(worldTilePos);

        public T SetMarker<T>(Vector2Int worldTilePos) where T : MarkerBase
        {
            var worldPos = DungeonManager.Instance.TilePosToWorld(worldTilePos);
            if (markers.TryGetValue(worldTilePos, out var _))
            {
                return null;
            }
            var marker = pool.Get<T>();

            marker.transform.position = worldPos;

            markers.Add(worldTilePos, marker);
            marker.Init(worldTilePos);

            return marker as T;
        }

        public bool ResetMarker(Vector2Int worldTilePos)
        {
            if (markers.Remove(worldTilePos, out var marker))
            {
                pool.Release(marker);
                return true;
            }
            return false;
        }

        public void ResetAllMarker()
        {
            if (markers.Count == 0)
            {
                return;
            }

            foreach (var marker in markers.Values)
            {
                pool.Release(marker);
            }
            markers.Clear();

        }
        private void OnTileChanged(DungeonManager.TileChangedInfo info)
        {
            if (this!=null&&!gameObject.activeSelf)
                return;

            foreach (var key in markers.ToArray())
            {
                key.Value.TileChanged(info);
            }
        }
    }
}
