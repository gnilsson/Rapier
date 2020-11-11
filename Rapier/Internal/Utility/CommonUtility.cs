using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rapier.Internal.Utility
{
    public static class CommonUtility
    {
        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> current,
            IDictionary<TKey, TValue> target)
            where TKey : notnull
        {
            foreach (var value in target)
                current.Add(value.Key, value.Value);
            return current;
        }

        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> current,
            IEnumerable<KeyValuePair<TKey, TValue>> target)
            where TKey : notnull
        {
            foreach (var value in target)
                current.Add(value.Key, value.Value);
            return current;
        }

        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> current,
            params (TKey, TValue)[] target)
            where TKey : notnull
        {
            foreach (var value in target)
                current.Add(value.Item1, value.Item2);
            return current;
        }

        public static string AppendQueryString(this string uri, string queryName, string queryValue)
        {
            return QueryHelpers.AddQueryString(uri, queryName, queryValue);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string ToLongDateTimeString(this DateTime dateTime)
            => dateTime.ToString("dddd, dd MMMM yyyy HH:mm");

    }
}
