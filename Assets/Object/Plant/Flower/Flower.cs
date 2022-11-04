using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class Flower : Plant
    {
        Vision vision;
        [SerializeField]
        float atk = 5;
        [SerializeField]
        float attackImpulse = 5;

        void Start()
        {
            Init();
            vision = GetComponentInChildren<Vision>();
        }

        private void Update()
        {
            if (IsFullGrowth && vision.ShouldFoundTarget)
            {

                foreach (var target in vision.Targets.Reverse())
                {
                    if (target.TryGetComponent<Scorpion>(out var scorpion))
                    {
                        Debug.Log("sc damage!");
                        scorpion.Damaged(atk, (transform.position - target.position).normalized * attackImpulse);
                    }
                    if (target.TryGetComponent<Spines>(out var spines))
                    {
                        Debug.Log("sp destroy!");
                        Destroy(spines.gameObject);
                    }

                    if (target.TryGetComponent<cactus>(out var cactus))
                    {
                        Debug.Log("ca damage!");
                        cactus.Damaged(atk);
                    }

                }
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        var pos = new Vector3Int(TilePos.x + i, TilePos.y + j, 0);
                        DungeonManager.Instance.ForceChange(pos, PlantType.Foundation, TilePos == pos);
                    }
                }

            }
        }
    }
}
