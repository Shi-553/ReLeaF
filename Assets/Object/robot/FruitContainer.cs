using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FruitContainer : MonoBehaviour
{
    [SerializeField]
    float margin = 0.5f;
    PositionConstraint _positionConstraint;
    PositionConstraint PositionConstraint {get
        {
            if( _positionConstraint == null)
                TryGetComponent(out _positionConstraint);
            return _positionConstraint;
        }
    }

    public void Connect(Transform source)
    {
        for (int i = 0; i < PositionConstraint.sourceCount; i++)
        {
            PositionConstraint.RemoveSource(i);
        }
        PositionConstraint.AddSource(new ConstraintSource() { sourceTransform = source, weight = 1 });
        
    }
    public int FruitCount()
    {
        return transform.childCount;
    }
    public bool IsEmpty()
    {
        return transform.childCount == 0;
    }
    public void Push(Transform fruit)
    {
        fruit.SetParent(transform);
        fruit.localPosition = GetFruitPos(transform.childCount-1);
    }
    public bool Pop(out Transform f)
    {
        if (transform.childCount == 0)
        {
            f = null;
            return false;
        }
        f=transform.GetChild(0);
        f.SetParent(null);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = GetFruitPos(i);
        }
        return true;
    }

    Vector3 GetFruitPos(int index)
    {
        return new Vector3(0,index* margin, 0);
    }
    public void Clear()
    {
        foreach (Transform t in transform)
        {
            UnityEngine.Object.Destroy(t.gameObject);
        }
    }
}
