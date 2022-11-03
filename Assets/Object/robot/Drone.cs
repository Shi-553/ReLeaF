using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReLeaf
{
    public class Drone : MonoBehaviour
    {
        [SerializeField]
        float speed = 3;
        [SerializeField]
        float range = 0.1f;

        PlantType plantType;


        public IEnumerator SowSeed(IEnumerable<Foundation> fs, PlantType type)
        {
            plantType = type;

            foreach (var f in fs)
            {
                while (true)
                {
                    if (f == null)
                    {
                        break;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, f.transform.position, speed * Time.deltaTime * DungeonManager.CELL_SIZE);

                    yield return null;

                    if (f == null)
                    {
                        break;
                    }

                    if ((f.transform.position - transform.position).sqrMagnitude < range * range)
                    {
                        f.SetHighlight(false);
                        DungeonManager.Instance.SowSeed(f.transform.position, plantType);
                        break;
                    }
                }
            }
            Destroy(gameObject);
        }

    }
}