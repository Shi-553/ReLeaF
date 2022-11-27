using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public static class MathExtension
    {
        static public float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        }

        static public float LerpPairs(SortedList<float, float> pairs, float t)
        {
            var old = pairs.First();
            t = Mathf.Clamp(t, old.Key, pairs.Last().Key);


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
        public class RandomIndex
        {
            readonly public float totalWeight;
            public float TotalWeight => totalWeight;
            readonly float[] weightTable;
            public RandomIndex(params float[] weightTable)
            {
                totalWeight = weightTable.Sum();
                this.weightTable = weightTable;
            }
            public int Get()
            {
                var value = Random.Range(0, TotalWeight);
                for (var i = 0; i < weightTable.Length; ++i)
                {
                    if (weightTable[i] >= value)
                    {
                        return i;
                    }
                    value -= weightTable[i];
                }

                throw new System.Exception("���肦���");
            }
        }

        // ���������f�t�H���g�Ƃ��郍�[�J�����W�������ɉ����ĉ�]
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

        public static bool DuringExists(Vector2Int target, Vector2Int start, Vector2Int end)
        {
            var xt = start.x < end.x ? (start.x, end.x) : (end.x, start.x);
            var yt = start.y < end.y ? (start.y, end.y) : (end.y, start.y);

            for (int x = xt.Item1; x <= xt.Item2; x++)
            {
                for (int y = yt.Item1; y <= yt.Item2; y++)
                {
                    if (target.x == x && target.y == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static Vector2Int ClampOneMagnitude(this Vector2 value)
        {
            if (Mathf.Abs(value.x) < Mathf.Abs(value.y))
            {
                return new Vector2Int(0, (value.y < 0 ? -1 : 1));
            }
            else
            {
                return new Vector2Int((value.x < 0 ? -1 : 1), 0);
            }
        }
        public static Vector2Int ClampOneMagnitude(this Vector2Int value)
        {
            return ClampOneMagnitude((Vector2)value);
        }
    }
}