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
    [SerializeField]
    float knockBackDampingRate = 0.9f;

    Vector2 move;
    Rigidbody2D rigid;
    void Start()
    {
        footTransform=transform.Find("Foot");
        hp = hpMax;
        TryGetComponent(out rigid);
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + move);
        move = Vector2.zero;
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

            add.Normalize();
            move.x += add.x * DungeonManager.CELL_SIZE.x * speed * Time.deltaTime;
            move.y += add.y * DungeonManager.CELL_SIZE.y * speed * Time.deltaTime;



            if (Input.GetKeyDown(KeyCode.Return))
            {
                robot.CollectFruit(add);
            }

            dungeonManager.SowSeed(footTransform.position, SeedType.Normal);
        }
    }

    public void Damaged(int damage,Vector3 impulse)
    {
        if(hp==0)
            return;

        hp-=damage;
        StartCoroutine(KnockBack(impulse));
        if (hp <= 0)
        {
            hp = 0;
            StartCoroutine(Death());
        }
    }
    IEnumerator KnockBack(Vector3 impulse)
    {
        while (true)
        {


            move.x += impulse.x * DungeonManager.CELL_SIZE.x * Time.deltaTime;
            move.y += impulse.y * DungeonManager.CELL_SIZE.y * Time.deltaTime;


            impulse *= knockBackDampingRate;

            if (impulse.sqrMagnitude < 0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator Death()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Return));
        SceneManager.LoadScene(0);
    }
}
