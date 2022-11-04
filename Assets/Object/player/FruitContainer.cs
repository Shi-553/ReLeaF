using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReLeaf
{
    public class FruitContainer : MonoBehaviour
    {
        [SerializeField]
        float margin = 0.5f;
        [SerializeField]
        Text text;
        public int FruitCount()
        {
            return transform.childCount;
        }
        public bool IsEmpty()
        {
            return transform.childCount == 0;
        }
        public void Push(Transform fruit)
        {
            fruit.SetParent(transform);
            fruit.localPosition = GetFruitPos(transform.childCount - 1);
            text.text = FruitCount().ToString();
        }
        public bool Pop(out Transform f)
        {
            if (transform.childCount == 0)
            {
                f = null;
                return false;
            }
            f = transform.GetChild(0);
            f.SetParent(null);

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = GetFruitPos(i);
            }
            text.text = FruitCount().ToString();
            return true;
        }

        Vector3 GetFruitPos(int index)
        {
            return new Vector3(0, index * margin, 0);
        }
        public void Clear()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            text.text = FruitCount().ToString();
        }
    }
}