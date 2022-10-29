using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField]
    int hpMax=2;
    [SerializeField]
    int hp=2;

    void Start()
    {
        hp=hpMax;
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            DungeonManager.Instance.Messy(transform.position);
            Destroy(gameObject);
        }
    }
}
