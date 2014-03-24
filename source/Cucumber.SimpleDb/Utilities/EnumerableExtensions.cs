using System;
using System.Collections.Generic;
using System.Linq;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TElement>> GroupsOf<TElement>(this IEnumerable<TElement> source, int groupSize)
        {
            if (groupSize < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            //this is not efficient. need to refactor to stream elements
            var enumerator = source.GetEnumerator();
            var group = new List<TElement>();
            while (enumerator.MoveNext())
            {
                if (group.Count == groupSize)
                {
                    yield return group;
                    group = new List<TElement>();
                }
                group.Add(enumerator.Current);
            }
            if (group.Count > 0)
            {
                yield return group;
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static TOutput SelectFirst<TInput, TOutput>(this IEnumerable<TInput> source, Func<TInput, TOutput> selector)
        {
            return source.Select(selector).First();
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            return source.Union(second, new GenericEqualityComparer<T>(comparer));
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(i => i);
        }

        public static IEnumerable<TElement> DistinctBy<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> distinctBy)
        {
            return source.Distinct(new KeyEqualityComparer<TElement, TKey>(distinctBy));
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(this IEnumerable<TElement> source, Func<TElement, TKey> keySelector, Func<TKey, TKey, bool> comparer)
        {
            return source.GroupBy(keySelector, new GenericEqualityComparer<TKey>(comparer));
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            return source.Intersect(second, new GenericEqualityComparer<T>(comparer));
        }

        private class GenericEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;

            public GenericEqualityComparer(Func<T, T, bool> comparer)
            {
                _comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        private class KeyEqualityComparer<TElement, TKey> : IEqualityComparer<TElement>
        {
            private readonly Func<TElement, TKey> _keySelector;

            public KeyEqualityComparer(Func<TElement, TKey> keySelector)
            {
                _keySelector = keySelector;
            }

            public bool Equals(TElement x, TElement y)
            {
                return _keySelector(x).Equals(_keySelector(y));
            }

            public int GetHashCode(TElement obj)
            {
                return _keySelector(obj).GetHashCode();
            }
        }
    }
}