using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robot : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    float nearDistance = 1;
    [SerializeField]
    float speed = 1;

    [SerializeField]
    int atk = 3;
    [SerializeField]
    float atkSpeed = 5;
    [SerializeField]
    float atkDuration = 1;

    bool isAttacked=false;
    void Start()
    {
        
    }


    public void CollectFruit(Vector3 dir)
    {
        StartCoroutine(WaitCollectFruit(dir));
    }
    IEnumerator WaitCollectFruit(Vector3 dir)
    {
        isAttacked = true;
        GetComponent<Collider2D>().isTrigger = true;

        float timer = 0;
        while (true)
        {
            transform.position+=dir* atkSpeed*Time.deltaTime;

            timer += Time.deltaTime;
            if (timer > atkDuration)
            {
                break;
            }
            yield return null;
        }

        isAttacked = false;
        GetComponent<Collider2D>().isTrigger = false;


        if (Vector3.Distance(transform.position, player.position) > nearDistance)
        {
            var add = (player.position - transform.position).normalized;

            transform.position += add * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacked)
        {
            var cactus = collision.gameObject.GetComponent<cactus>();
            cactus.Damaged(atk);
        }
    }
}
