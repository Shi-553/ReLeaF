using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        lifeTimeCounter = 0;
    }

    void Update()
    {
        lifeTimeCounter += Time.deltaTime;

        if (lifeTimeCounter >= lifeTime)
        {
            Destroy(gameObject);
        }

        transform.position += new Vector3(
            transform.up.x * speed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
            transform.up.y * speed * DungeonManager.CELL_SIZE.y * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerControler>(out var player))
            {
                player.Damaged(atk);
                Destroy(gameObject);
            }
        }
    }
}
