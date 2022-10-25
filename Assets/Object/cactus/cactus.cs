using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class cactus : MonoBehaviour
{
    [SerializeField]
    int hpMax = 3;
    [SerializeField]
    int hp = 3;
    [SerializeField]
    int explosionRange = 2;

    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    Tile tile;
    void Start()
    {
        hp = hpMax;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {

            for (int i = -explosionRange; i < explosionRange; i++)
            {
                for (int j = -explosionRange; j < explosionRange; j++)
                {
                    tilemap.SetTile(tilemap.WorldToCell(transform.position) + new Vector3Int(i, j, 0), tile);
                }
            }
            Destroy(gameObject);
        }
    }

}
