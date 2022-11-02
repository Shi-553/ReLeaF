using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public bool ShouldFoundTarget => Targets.Count != 0;

    [SerializeField]
    string[] targetTags = {"Player" };
    public HashSet<Transform> Targets { get; private set; }=new HashSet<Transform>();
    public Transform LastTargets { get; private set; }
    private void Start()
    {
        Targets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                Targets.Add( collision.transform);
                LastTargets= collision.transform;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                Targets.Remove(collision.transform);
            }
        }
    }
}
