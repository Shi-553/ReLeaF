using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{

    public abstract class Vision : MonoBehaviour
    {
        [SerializeField]
        string[] targetTags = { "Player" };

        [SerializeField]
        protected LayerMask mask = ~0;

        List<Transform> targets = new();
        Collider2D[] results;
        private void Start()
        {
            results = new Collider2D[GetInitCapacity()];
        }
        public IEnumerable<Transform> Targets()
        {
            foreach (var item in targets)
            {
                if (item != null)
                    yield return item.transform;
            }
        }

        protected abstract int GetInitCapacity();
        protected abstract Collider2D[] GetOverLapAll();

        void UpdateResult()
        {
            targets.Clear();
            foreach (var collider in results)
            {
                if (targetTags.Any(t => collider.CompareTag(t)))
                {
                    targets.Add(collider.transform);
                }
            }
        }

        public bool UpdateTarget()
        {
            results = GetOverLapAll();
            UpdateResult();

            return targets.Any();
        }
    }
}