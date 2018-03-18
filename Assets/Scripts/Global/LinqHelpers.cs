using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqHelpers
{
    public static TSource Random<TSource>(this IEnumerable<TSource> source)
    {

        return source.ElementAt(UnityEngine.Random.Range(0, source.Count()));
    }

    public static TSource Random<TSource>(this IEnumerable<TSource> source,
                                          Func<TSource, bool> predicate)
    {
        return Random(source.Where(predicate));
    }

    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source)
    {
        return Random(source.DefaultIfEmpty(default(TSource)));
    }

    public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source,
                                          Func<TSource, bool> predicate)
    {
        return RandomOrDefault(source.Where(predicate));
    }
}
