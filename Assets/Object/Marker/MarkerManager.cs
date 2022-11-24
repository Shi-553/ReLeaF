using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace ReLeaf
{
    public class MarkerManager : MonoBehaviour
    {

        Dictionary<Vector2Int, MarkerBase> markers = new();

        public ReadOnlyDictionary<Vector2Int, MarkerBase> Markers;

        [SerializeField]
        bool subscribeOnTileChanged;

        IPool pool;
        IPool GetPool<T>()where T:MarkerBase => pool ??= ComponentPool.Instance.SetPool(marker as T);

        [SerializeField]
        MarkerBase marker;

        private void Awake()
        {
            Markers = new ReadOnlyDictionary<Vector2Int, MarkerBase>(markers);
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



        public T SetMarker<T>(Vector2Int worldTilePos) where T : MarkerBase
        {
            var worldPos = DungeonManager.Instance.TilePosToWorld(worldTilePos);
            if (markers.TryGetValue(worldTilePos, out var _))
            {
                return null;
            }
            var marker= GetPool<T>().Get<T>(marker=> marker.transform.position = worldPos);

            markers.Add(worldTilePos, marker);


            return marker;
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
                if (marker != null)
                    pool.Release(marker);
            }
            markers.Clear();

        }
        private void OnTileChanged(DungeonManager.TileChangedInfo info)
        {
            if (this != null && !gameObject.activeSelf)
                return;

            foreach (var key in markers.ToArray())
            {
                key.Value.TileChanged(info);
            }
        }
    }
}
