using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtension
{
    // https://stackoverflow.com/questions/914109/how-to-use-linq-to-select-object-with-minimum-or-maximum-property-value
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector)
    {
        return source.MinBy(selector, null);
    }

    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> selector, IComparer<TKey> comparer)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (selector == null) throw new ArgumentNullException("selector");
        comparer ??= Comparer<TKey>.Default;

        using (var sourceIterator = source.GetEnumerator())
        {
            if (!sourceIterator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
            var min = sourceIterator.Current;
            var minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0)
                {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }
            return min;
        }
    }
}
