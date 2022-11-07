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

        IEnumerable<Foundation> foundations;
        Coroutine co;
        public void SowSeed(IEnumerable<Foundation> fs)
        {
            foundations = fs;
            co = StartCoroutine(SowSeedEnumerator());
        }

        public void Stop()
        {
            if (co != null)
            {
                foreach (var f in foundations.Where(f=>f!=null))
                {
                    f.ReSetSowSchedule();
                }
                StopCoroutine(co);
                co = null;
            }
            Destroy(gameObject);
        }
        public IEnumerator SowSeedEnumerator()
        {

            foreach (var f in foundations)
            {
                while (true)
                {
                    if (f == null || !f.IsSowScheduled)
                    {
                        break;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, f.transform.position, speed * Time.deltaTime * DungeonManager.CELL_SIZE);

                    if ((f.transform.position - transform.position).sqrMagnitude < range * range)
                    {
                        DungeonManager.Instance.SowSeed(f.TilePos, f.SowScheduledPlantType);
                        break;
                    }

                    yield return null;

                }
            }
            Destroy(gameObject);
        }

    }
}