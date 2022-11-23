using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class Vision : MonoBehaviour
    {
        public bool ShouldFoundTarget => Targets.Any();

        [SerializeField]
        string[] targetTags = { "Player" };
        HashSet<Transform> targets = new HashSet<Transform>();
        public IEnumerable<Transform> Targets => targets.Where(t => t != null);

        public Transform LastTarget { get; private set; }
        private void Start()
        {
            targets.Clear();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            foreach (var tag in targetTags)
            {
                if (collision.CompareTag(tag))
                {
                    targets.Add(collision.transform);
                    LastTarget = collision.transform;
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            foreach (var tag in targetTags)
            {
                if (collision.CompareTag(tag))
                {
                    targets.Remove(collision.transform);
                }
            }
        }

    }
}