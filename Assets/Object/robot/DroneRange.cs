using Mono.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DroneRange : MonoBehaviour
{
    HashSet<Transform> fruits = new HashSet<Transform>();
    public IReadOnlyCollection<Transform> TargetFruits => fruits;

    public bool IsTargeting { get; private set; }

    private void Start()
    {
        fruits.Clear();
    }
    public void BeginTarget(Transform center)
    {
        fruits.Clear();
        gameObject.SetActive(true);
        IsTargeting = true;
        GetComponent<PositionConstraint>().AddSource(new ConstraintSource() { sourceTransform = center, weight = 1 });
    }
    public void EndTarget()
    {
        IsTargeting = false;
        GetComponent<PositionConstraint>().RemoveSource(0);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsTargeting)
        {
            return;
        }
        if (collision.CompareTag("Fruit"))
        {
            var f = collision.GetComponent<Fruit>();
            if (!f.IsAttack && fruits.Add(collision.transform))
            {
                f.Highlight(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsTargeting)
        {
            return;
        }
        if (collision.CompareTag("Fruit"))
        {
            var f = collision.GetComponent<Fruit>();
            if (!f.IsAttack && fruits.Remove(collision.transform))
            {
                f.Highlight(false);
            }
        }
    }
}
