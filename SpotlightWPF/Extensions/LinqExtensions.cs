using System;
using System.Collections.Generic;

namespace Spotlight.Extensions
{
    public static class LinqExtensions
    {
        // Source: https://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property/489421#489421
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
