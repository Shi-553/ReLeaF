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
        float reGrowTime = 1;
        [SerializeField,ReadOnly]
        float reGrowTimeCounter = 0;

        [SerializeField]
        Fruit fruitPrefab;

        void Start()
        {
            Init();
            vision = GetComponentInChildren<Vision>();
            reGrowTimeCounter = reGrowTime;
        }

        private void Update()
        {
            if (!IsFullGrowth)
            {
                return;
            }
            if (reGrowTime > reGrowTimeCounter)
            {
                reGrowTimeCounter += Time.deltaTime;
            }
            else
            {
                if (vision.ShouldFoundTarget)
                {
                    reGrowTimeCounter = 0;
                    var target = vision.Targets.MinBy(t => (t.position - transform.position).sqrMagnitude);

                    Vector2 dir = target.position - transform.position;
                    dir.Normalize();

                    var f=Instantiate(fruitPrefab,transform);
                    f.Shot(dir);
                }
            }
        }
    }
}
