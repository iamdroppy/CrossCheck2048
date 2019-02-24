using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class Extensions
    {
        public static T RandomElement<T>(this IEnumerable<T> enumerable, Random random)
        {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray(); // prevent multiple iterations on the IEnumerable.
            return enumerable1.ElementAt(random.Next(0, enumerable1.Count()));
        }
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
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
