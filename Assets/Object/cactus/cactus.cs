using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class cactus : MonoBehaviour, IRoomEnemy
{
    [SerializeField]
    int hpMax = 3;
    [SerializeField]
    int hp = 3;

    [SerializeField]
    bool shouldAimTarget;

    [SerializeField]
    int attackSpinesNum = 1;

    [SerializeField]
    GameObject spinesPrefab;
    [SerializeField]
    float attackTime = 2.0f;

    Vision vision;

    float attackTimeCounter = 0;
    public bool CanAttackPlayer { get; set; }

    void Start()
    {
        attackTimeCounter = 0;
        hp = hpMax;
        vision = GetComponentInChildren<Vision>();
    }


    void Update()
    {
        if (!CanAttackPlayer)
        {
            return;
        }
        if (vision.ShouldFoundTarget || attackTimeCounter > 0)
        {
            attackTimeCounter += Time.deltaTime;


            transform.localScale =Vector3.one*MathExtension.LerpPairs(new SortedList<float, float>{ {0,1 }, { 0.8f, 1 },{ 0.95f, 0.8f }, { 1, 1 } },attackTimeCounter/attackTime);
            

            if (vision.ShouldFoundTarget&&attackTimeCounter >= attackTime)
            {
                attackTimeCounter = 0;

                var rotation = shouldAimTarget ?
                    Quaternion.FromToRotation(Vector3.up, vision.Targets.MinBy(t=>(t.transform.position-transform.position).sqrMagnitude).position - transform.position) :
                    Quaternion.identity;
                for (int i = 0; i < attackSpinesNum; i++)
                {
                    Instantiate(spinesPrefab, transform.position, rotation * Quaternion.Euler(0, 0, 360.0f * i / attackSpinesNum));
                }
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
