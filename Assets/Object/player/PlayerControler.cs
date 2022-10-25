using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
        Tilemap tilemap;
    [SerializeField]
    TerrainTile tile;

    [SerializeField]
    float speed=1.0f;

    [SerializeField]
    robot robot;

    void Start()
    {
        //tilemap=
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tilemap.SetTile(tilemap.WorldToCell(transform.position), tile);
            
        }

        Vector2 add = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            add += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            add += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            add += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            add += Vector2.right;
        }
        if (add != Vector2.zero)
        {
            var pos = transform.position;

            add.Normalize();
            pos.x += add.x * tilemap.cellSize.x * speed*Time.deltaTime ;
            pos.y += add.y * tilemap.cellSize.x * speed * Time.deltaTime;

            transform.position = pos;


            if (Input.GetKeyDown(KeyCode.Return))
            {
                robot.Attack(add);
            }
        }
    }
}
