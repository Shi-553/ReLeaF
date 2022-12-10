using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public class MarkerManager : MonoBehaviour
    {

        Dictionary<Vector2Int, MarkerBase> markers = new();

        public ReadOnlyDictionary<Vector2Int, MarkerBase> Markers;

        [SerializeField]
        bool subscribeOnTileChanged;

        IPool pool;
        IPool GetPool<T>() where T : MarkerBase => pool ??= PoolManager.Singleton.SetPool(marker as T);

        [SerializeField]
        MarkerBase marker;

        private void Awake()
        {
            Markers = new ReadOnlyDictionary<Vector2Int, MarkerBase>(markers);
        }

        private void Start()
        {
            if (subscribeOnTileChanged)
                DungeonManager.Singleton.OnGreening += OnGreening;
        }

        private void OnDestroy()
        {
            if (subscribeOnTileChanged)
                DungeonManager.Singleton.OnGreening -= OnGreening;
            ResetAllMarker();
        }



        public T SetMarker<T>(Vector2Int worldTilePos) where T : MarkerBase
        {
            return SetMarker<T>(worldTilePos, Quaternion.identity);
        }
        public T SetMarker<T>(Vector2Int worldTilePos, Quaternion rotation) where T : MarkerBase
        {
            var worldPos = DungeonManager.Singleton.TilePosToWorld(worldTilePos);
            if (markers.TryGetValue(worldTilePos, out var _))
            {
                return null;
            }
            using var _ = GetPool<T>().Get<T>(out var marker);

            marker.transform.position = worldPos;
            marker.transform.rotation = rotation;

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
        private void OnGreening(DungeonManager.GreeningInfo info)
        {
            if (this != null && !gameObject.activeSelf)
                return;

            foreach (var key in markers.ToArray())
            {
                key.Value.OnGreening(info);
            }
        }
    }
}
