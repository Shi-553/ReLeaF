using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathExtension
{
    static public float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }

    static public float LerpPairs(SortedList<float, float> pairs, float t)
    {
        var old = pairs.First();
        t = Mathf.Clamp(t,old.Key,pairs.Last().Key);


        foreach (var p in pairs)
        {
            if (p.Key <= t)
            {
                old = p;
                continue;
            }
            var tt = Map(t, old.Key, p.Key, 0, 1);
            return Mathf.Lerp(old.Value, p.Value, tt);
        }

        return pairs.Last().Value;
    }
    public static int GetRandomIndex(params float[] weightTable)
    {
        var totalWeight = weightTable.Sum();
        var value = Random.Range(0, totalWeight );
        for (var i = 0; i < weightTable.Length; ++i)
        {
            if (weightTable[i] >= value)
            {
                return i;
            }
            value -= weightTable[i];
        }

        throw new System.Exception("ありえんわ");
    }

    // 下向きをデフォルトとするローカル座標を向きに応じて回転
    public static Vector2Int GetRotatedLocalPos(Vector2Int dir, Vector2Int defaultLocal)
    {
        if (dir == Vector2Int.up)
        {
            return defaultLocal;
        }
        if (dir == Vector2Int.down)
        {
            return -defaultLocal;
        }
        if (dir == Vector2Int.left)
        {
            return -new Vector2Int(defaultLocal.y, defaultLocal.x);
        }
        if (dir == Vector2Int.right)
        {
            return new Vector2Int(defaultLocal.y, defaultLocal.x);
        }

        return defaultLocal;
    }

    public static (float, float) MinMax(float value1, float value2)
    {
        if (value1 < value2)
            return (value1, value2);
        else
            return (value2, value1);
    }
    public static (int, int) MinMax(int value1, int value2)
    {
        if (value1 < value2)
            return (value1, value2);
        else
            return (value2, value1);
    }
    public static (Vector2, Vector2) MinMax(Vector2 value1, Vector2 value2)
    {
        var x=MinMax(value1.x,value2.x);
        var y=MinMax(value1.y,value2.y);

        return (new Vector2(x.Item1, y.Item1), new Vector2(x.Item2, y.Item2));
    }
    public static (Vector2Int, Vector2Int) MinMax(Vector2Int value1, Vector2Int value2)
    {
        var x=MinMax(value1.x,value2.x);
        var y=MinMax(value1.y,value2.y);

        return (new Vector2Int(x.Item1, y.Item1), new Vector2Int(x.Item2, y.Item2));
    }
}
