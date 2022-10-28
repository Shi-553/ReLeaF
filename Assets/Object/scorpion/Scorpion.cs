using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : MonoBehaviour
{
    [SerializeField]
    int hpMax = 3;
    [SerializeField]
    int hp = 3;
    [SerializeField]
    float speed = 2.0f;
    [SerializeField]
    int atk = 1;
    [SerializeField]
    float attackCoolTime = 1.0f;
    float attackCoolTimeCounter = 0;
    [SerializeField]
    float attackAimTime = 1.0f;
    [SerializeField]
    float attackDuration = 1.0f;
    [SerializeField]
    float attackSpeed = 4.0f;

    Vision searchVision;
    Vision attackVision;

    bool isAttack = false;
    void Start()
    {
        hp = hpMax;
        isAttack = false;
        transform.Find("AttackVision").TryGetComponent(out attackVision);
        transform.Find("SearchVision").TryGetComponent(out searchVision);
    }

    void Update()
    {
        if (attackCoolTimeCounter > 0)
        {
            attackCoolTimeCounter -= Time.deltaTime;
            return;
        }

        if (isAttack)
            return;

        if (attackVision.ShouldFoundTarget)
        {
            isAttack = true;
            StartCoroutine(Attack());
            return;
        }

        if (!searchVision.ShouldFoundTarget)
        {
            return;
        }
        var dir = (searchVision.Target.position - transform.position).normalized;

        transform.position += new Vector3(
            dir.x * speed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
            dir.y * speed * DungeonManager.CELL_SIZE.y * Time.deltaTime);
    }

    IEnumerator Attack()
    {
        float counter = 0;
        while (attackAimTime > counter)
        {
            transform.localScale = Vector3.one * MathExtension.LerpPairs(
                    new SortedList<float, float> { { 0, 1 }, { 1, 0.8f } }, counter / attackAimTime);

            counter += Time.deltaTime;
            yield return null;
        }
        counter = 0;


        var dir = (searchVision.Target.position - transform.position).normalized;

        while (attackDuration > counter)
        {
            transform.localScale = Vector3.one * MathExtension.LerpPairs(
                    new SortedList<float, float> { { 0, 0.8f }, { 0.1f, 1.0f }, { 1, 1 } }, counter / attackDuration);

            transform.position += new Vector3(
                dir.x * attackSpeed * DungeonManager.CELL_SIZE.x * Time.deltaTime,
                dir.y * attackSpeed * DungeonManager.CELL_SIZE.y * Time.deltaTime);

            counter += Time.deltaTime;
            yield return null;
        }
        isAttack = false;
        attackCoolTimeCounter = attackCoolTime;
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
    public void Damaged(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
