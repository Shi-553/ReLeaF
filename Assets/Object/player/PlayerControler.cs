using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    DungeonManager dungeonManager;

    [SerializeField]
    float speed = 1.0f;

    [SerializeField]
    robot robot;

    Transform footTransform;

    void Start()
    {
        footTransform=transform.Find("Foot");
    }

    void Update()
    {

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
            pos.x += add.x * dungeonManager.GetCellSize().x * speed * Time.deltaTime;
            pos.y += add.y * dungeonManager.GetCellSize().x * speed * Time.deltaTime;

            transform.position = pos;


            if (Input.GetKeyDown(KeyCode.Return))
            {
                robot.CollectFruit(add);
            }

            dungeonManager.SowSeed(footTransform.position, SeedType.Normal);
        }
    }
}
