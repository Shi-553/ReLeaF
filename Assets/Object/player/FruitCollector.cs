using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    public class FruitCollector : MonoBehaviour
    {
        [SerializeField]
        Vision collectVision;

        PlayerController player;
        void Start()
        {
            TryGetComponent(out player);
        }

        void Update()
        {
            foreach (var t in collectVision.Targets)
            {
                if (t.TryGetComponent<Shrub>(out var shrub)&&shrub.Harvest(out var fruit))
                {
                    player.FruitContainer.Push(fruit.transform);
                }
            }

        }
    }
}