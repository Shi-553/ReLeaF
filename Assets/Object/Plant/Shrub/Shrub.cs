using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Shrub : Plant
{
    [SerializeField]
    GameObject fruitPrefab;

    [SerializeField]
    float regrowFruitTime=8;

    Fruit fruit;

    private void Start()
    {
        Init();
    }

    protected override void FullGrowed()
    {
        InstanceFruit();
    }
    public bool Harvest(out Fruit f)
    {
        if (fruit == null)
        {
            f = null;
            return false;
        }

        StartCoroutine(RegrowFruit());
        f = fruit;
        fruit = null;

        return true;
    }

    // もう一度実を付けるまで
    IEnumerator RegrowFruit()
    {
        yield return new WaitForSeconds(regrowFruitTime);
        InstanceFruit();
    }

    void InstanceFruit()
    {
         var fruitObj = Instantiate(fruitPrefab, transform.position, Quaternion.identity);
        fruitObj.TryGetComponent(out fruit);
    }

}
