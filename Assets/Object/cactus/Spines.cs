using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Spines : MonoBehaviour
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
    void Start()
    {
        lifeTimeCounter = 0;
        TryGetComponent(out rigid);
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + move);
        move = Vector2.zero;
    }
    void Update()
    {
        lifeTimeCounter += Time.deltaTime;

        if (lifeTimeCounter >= lifeTime)
        {
            Destroy(gameObject);
        }

        move += new Vector2(
            transform.up.x * speed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
            transform.up.y * speed * DungeonManager.CELL_SIZE.y * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerControler>(out var player))
            {
                player.Damaged(atk, transform.up * attackKnockBackPower);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Plant"))
        {
            if (collision.gameObject.TryGetComponent<Plant>(out var plant))
            {
                plant.Damaged(atk);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
