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
    FruitContainer fruitContainer;

    void Start()
    {

    }
    public IEnumerator Harvest(IEnumerable<Transform> fEnumerable)
    {
        transform.position = player.transform.position;

        fruitContainer.transform.position = transform.position;
        fruitContainer.Connect(transform);


        var fruitTransforms = fEnumerable.ToList();

        while (fruitTransforms.Count > 0)
        {
            fruitTransforms = fruitTransforms.Where(f => f != null).ToList();
            var nearFruit = fruitTransforms.MinBy(f => (f.position - transform.position).sqrMagnitude);
            fruitTransforms.Remove(nearFruit);

            while (true)
            {
                transform.position=Vector3.MoveTowards(transform.position, nearFruit.position, speed * DungeonManager.CELL_SIZE.x * Time.deltaTime);
               

                if ((transform.position - nearFruit.position).sqrMagnitude < collectionRadius* collectionRadius)
                {
                    dungeonManager.Harvest(nearFruit.position);
                    fruitContainer.Push(nearFruit);
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
        player.Harvested(fruitContainer);
    }
}
