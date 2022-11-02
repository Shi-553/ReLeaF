using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foundation : Plant
{
    private void Start()
    {
        Init();
    }

    protected override void FullGrowed()
    {
        plantObjRoot.SetActive(true);
    }
}
