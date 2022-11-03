using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSeed : MonoBehaviour
{
    [SerializeField]
    Transform root;
    [SerializeField]
    Transform pointer;

    List<SeedUI> seeds = new List<SeedUI>();
    int selectIndex = 0;

    public SeedUI CurrentSeed => seeds[selectIndex];

    void Start()
    {
        for (int i = 0; i < root.childCount; i++)
        {
            seeds.Add(root.GetChild(i).GetComponent<SeedUI>());
        }
        selectIndex = 0;
        UpdatePointer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectIndex = (selectIndex - 1+ seeds.Count) % seeds.Count;
            UpdatePointer();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectIndex = (selectIndex + 1) % seeds.Count;
            UpdatePointer();

        }
    }
    void UpdatePointer()
    {
        pointer.position = seeds[selectIndex].transform.position;
    }
}
