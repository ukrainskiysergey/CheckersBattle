using System;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    public static T MinBy<T>(this IEnumerable<T> enumerable, Func<T, IComparable> selector) where T : class
    {
        var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext()) return null;
        var minKey = enumerator.Current;
        var minValue = selector(minKey);

        while (enumerator.MoveNext())
        {
            var key = enumerator.Current;
            var value = selector(key);
            if (value.CompareTo(minValue) < 0)
            {
                minKey = key;
                minValue = value;
            }
        }

        return minKey;
    }

}