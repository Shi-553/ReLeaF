using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public bool IsHarvested { get; private set; } 

    void Start()
    {
        IsHarvested = false;
    }

    public void Harvest()
    {
        IsHarvested = true;

    }
    public void Destroy()
    {
        if(gameObject != null&& !IsHarvested)
        Destroy(gameObject);
    }
}
