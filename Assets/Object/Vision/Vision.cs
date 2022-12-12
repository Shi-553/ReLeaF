using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{

    public abstract class Vision : MonoBehaviour
    {
        [SerializeField]
        string[] targetTags = { "Player" };

        Transform[] targets;

        public IEnumerable<Transform> Targets()
        {
            foreach (var item in targets)
            {
                if (item != null)
                    yield return item.transform;
            }
        }

        protected abstract Collider2D[] GetOverLapAll();

        public bool UpdateTarget()
        {
            targets = GetOverLapAll()
                .Where(item =>
                targetTags.Any(t => item.CompareTag(t)))
                .Select(c => c.transform)
                .ToArray();

            return targets.Any();
        }
    }
}