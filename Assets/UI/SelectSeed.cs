using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class SelectSeed : MonoBehaviour
    {
        [SerializeField]
        Transform root;
        [SerializeField]
        Transform pointer;

        List<SeedUI> seeds = new List<SeedUI>();
        int selectIndex = 0;

        public SeedUI CurrentSeed => seeds[selectIndex];


        public void MoveSelect(float move)
        {
            selectIndex = (selectIndex - Mathf.CeilToInt(move) + seeds.Count) % seeds.Count;
            UpdatePointer();
        }

        void Start()
        {
            for (int i = 0; i < root.childCount; i++)
            {
                seeds.Add(root.GetChild(i).GetComponent<SeedUI>());
            }
            selectIndex = 0;
            UpdatePointer();
        }

        void UpdatePointer()
        {
            pointer.position = seeds[selectIndex].transform.position;
        }
    }
}