using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    int hpMax = 10;
    [SerializeField]
    int hp = 10;

    void Start()
    {
        footTransform=transform.Find("Foot");
        hp = hpMax;
    }

    void Update()
    {
        if (hp == 0)
        {
            return;
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
            pos.x += add.x * DungeonManager.CELL_SIZE.x * speed * Time.deltaTime;
            pos.y += add.y * DungeonManager.CELL_SIZE.y * speed * Time.deltaTime;

            transform.position = pos;


            if (Input.GetKeyDown(KeyCode.Return))
            {
                robot.CollectFruit(add);
            }

            dungeonManager.SowSeed(footTransform.position, SeedType.Normal);
        }
    }

    public void Damaged(int damage)
    {
        if(hp==0)
            return;

        hp-=damage;
        if (hp <= 0)
        {
            hp = 0;
            StartCoroutine(Death());
        }
    }
    IEnumerator Death()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Return));
        SceneManager.LoadScene(0);
    }
}
