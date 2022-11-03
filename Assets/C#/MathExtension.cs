using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    
}
