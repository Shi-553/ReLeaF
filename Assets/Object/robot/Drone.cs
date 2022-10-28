using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField]
    DungeonManager dungeonManager;
    [SerializeField]
    PlayerControler player;

    [SerializeField]
    float speed = 3;
    [SerializeField]
    float collectionRadius = 0.5f;
    [SerializeField]
    GameObject fruitContainerPrefab;

    void Start()
    {

    }
    public IEnumerator Harvest(IEnumerable<Transform> fEnumerable)
    {
        transform.position = player.transform.position;

        var containerObj=Instantiate(fruitContainerPrefab);
        containerObj.transform.position = transform.position;
        var container = containerObj.GetComponent<FruitContainer>();
        container.Connect(transform);


        var fruitTransforms = fEnumerable.ToList();
        var fruits = fruitTransforms.Select(f => dungeonManager.Harvest(f.position)).ToArray();

        while (fruitTransforms.Count > 0)
        {
            var nearFruit = fruitTransforms.MinBy(f => (f.position - transform.position).sqrMagnitude);
            fruitTransforms.Remove(nearFruit);

            while (true)
            {
                transform.position=Vector3.MoveTowards(transform.position, nearFruit.position, speed * DungeonManager.CELL_SIZE.x * Time.deltaTime);
               

                if ((transform.position - nearFruit.position).sqrMagnitude < collectionRadius* collectionRadius)
                {
                    container.Push(nearFruit);
                    break;
                }
                yield return null;
            }
        }

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * DungeonManager.CELL_SIZE.x * Time.deltaTime);


            if ((transform.position == player.transform.position))
            {
                break;
            }
            yield return null;
        }
        player.Harvested(container);
    }
}
