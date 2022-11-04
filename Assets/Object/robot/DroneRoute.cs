using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace ReLeaf
{
    public class DroneRoute : MonoBehaviour
    {
        List<Foundation> lastTargets = new List<Foundation>();
        public IReadOnlyCollection<Foundation> LastTargets => lastTargets;
        [SerializeField]
        float speed = 5;

        Vector2Int dir;
        public bool IsRouting { get; private set; }

        [SerializeField]
        float routingConsumeEnergy = 0.1f;
        [SerializeField]
        float sowSeedConsumeEnergy = 1;

        ValueGaugeManager energyPointManager;
        private void Awake()
        {
            energyPointManager = GetComponentInParent<ValueGaugeManager>();
        }
        public void SetDir(Vector2Int dir)
        {
            this.dir = dir;

        }
        public void End(bool resetHighlight)
        {
            IsRouting = false;
            gameObject.SetActive(false);
            if (resetHighlight)
            {
                foreach (var t in lastTargets)
                {
                    t.SetHighlight(false);
                }
            }
        }
        public IEnumerator Begin()
        {
            IsRouting = true;
            gameObject.SetActive(true);
            lastTargets.Clear();
            yield return StartCoroutine(Move());
        }
        private void Update()
        {
            if (IsRouting)
            {
                if (!energyPointManager.ConsumeValue(routingConsumeEnergy))
                {
                    IsRouting = false;
                }
            }
        }
        IEnumerator Move()
        {

            dir = Vector2Int.zero;
            var targetTilePos = DungeonManager.Instance.WorldToTilePos(transform.position);
            while (true)
            {
                var targetWorldPos = DungeonManager.Instance.TilePosToWorld(targetTilePos);
                var target = DungeonManager.Instance.GetGroundTileObject(targetTilePos);
                while (true)
                {
                    if (!IsRouting)
                    {
                        yield break;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, DungeonManager.CELL_SIZE * Time.unscaledDeltaTime * speed);

                    yield return null;

                    if (transform.position == targetWorldPos)
                    {
                        if (target != null && target.TryGetComponent<Foundation>(out var foundation) && foundation.IsFullGrowth && !foundation.IsHighlighting)
                        {
                            if (energyPointManager.ConsumeValue(sowSeedConsumeEnergy))
                            {
                                foundation.SetHighlight(true);
                                lastTargets.Add(foundation);
                            }
                            else
                            {
                                yield break;
                            }
                        }
                        break;
                    }
                }

                dir = Vector2Int.zero;
                yield return new WaitWhile(() => dir == Vector2Int.zero&& IsRouting);

                targetTilePos += (Vector3Int)dir;
            }
        }

    }
}