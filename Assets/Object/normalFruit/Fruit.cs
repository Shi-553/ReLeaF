using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField]
    float lifeTime = 2.0f;
    float lifeTimeCounter;

    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    int atk = 1;
    [SerializeField]
    float attackKnockBackPower = 4.0f;
    Rigidbody2D rigid;
    Vector2 move;

    [SerializeField]
    float diffusion = 10;

    public bool IsAttack { get; private set; }

    Vector3 dir;
    void Start()
    {
        IsAttack = false;
        lifeTimeCounter = 0;
        TryGetComponent(out rigid);
    }

    public void SteppedOn()
    {
        Destroy(gameObject);
    }
    public void Highlight(bool sw)
    {
        transform.GetChild(0).gameObject.SetActive(sw);
    }
    public void Shot(Vector3 dir)
    {
        this.dir = Quaternion.Euler(0, 0, Random.Range(-diffusion, diffusion)) * dir;
        IsAttack = true;
    }


    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + move);
        move = Vector2.zero;
    }
    void Update()
    {
        if (!IsAttack)
        {
            return;
        }

        lifeTimeCounter += Time.deltaTime;

        if (lifeTimeCounter >= lifeTime)
        {
            Destroy(gameObject);
        }

        move += new Vector2(
            dir.x * speed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
            dir.y * speed * DungeonManager.CELL_SIZE.y * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAttack)
        {
            return;
        }
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<cactus>(out var c))
            {
                c.Damaged(atk);
                Destroy(gameObject);
            }
            if (collision.TryGetComponent<Scorpion>(out var s))
            {
                s.Damaged(atk, dir * attackKnockBackPower);
                Destroy(gameObject);
            }
        }
    }
}
