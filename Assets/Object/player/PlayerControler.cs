using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    DungeonManager dungeonManager;

    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float shotMoveSpeed = 2;
    [SerializeField]
    float shotSpeed = 3;
    float shotTimeCounter = 0;

    [SerializeField]
    DroneManager droneManager;

    Transform footTransform;

    [SerializeField]
    int hpMax = 10;
    [SerializeField]
    int hp = 10;
    [SerializeField]
    float knockBackDampingRate = 0.9f;

    Vector2 move;
    Rigidbody2D rigid;
    FruitContainer fruitContainer;

    [SerializeField]
    Text text;

    void Start()
    {
        footTransform = transform.Find("Foot");
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0);
        }
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
            var speed = fruitContainer == null ? moveSpeed : shotMoveSpeed;

            move.x += add.x * DungeonManager.CELL_SIZE.x * speed * Time.deltaTime;
            move.y += add.y * DungeonManager.CELL_SIZE.y * speed * Time.deltaTime;




            dungeonManager.SowGrass(footTransform.position);
        }

        if (fruitContainer == null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                droneManager.SetupRange(transform);
            }
            if (Input.GetMouseButtonUp(1))
            {
                droneManager.Harvest();
            }
        }

        if (shotTimeCounter > 0.0f)
        {
            shotTimeCounter -= Time.deltaTime * shotSpeed;
        }
        else
        {
            if (fruitContainer != null && Input.GetMouseButton(0))
            {
                shotTimeCounter = 1.0f;

                if (fruitContainer.Pop(out var f))
                {
                    f.position = transform.position;
                    Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    dir.Normalize();

                    f.GetComponent<Fruit>().Shot(dir);
                    text.text = fruitContainer.FruitCount().ToString();
                }
                if (fruitContainer.IsEmpty())
                {
                    fruitContainer = null;
                }
            }
        }
    }

    public void Damaged(int damage, Vector3 impulse)
    {
        if (hp == 0)
            return;

        hp -= damage;
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
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SceneManager.LoadScene(0);
    }

    public void Harvested(FruitContainer container)
    {
        fruitContainer = container;
        fruitContainer.transform.position = transform.position;
        fruitContainer.Connect(transform);

        text.text = fruitContainer.FruitCount().ToString();
    }
    public void EnterRoom()
    {
        if (fruitContainer != null)
        {
            fruitContainer.Clear();
            fruitContainer = null;
        }
        droneManager.Cancel();
    }
}
