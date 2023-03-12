using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyZoneBuilder.Core
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Difference<T>( this IEnumerable<T> param1, IEnumerable<T> param2 )
        {
            HashSet<T> set2 = new HashSet<T>(param2);
            foreach ( T element in param1 )
            {
                if ( !set2.Contains(element) )
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T[]> SplitIntoChunks<T>( this T[] array, int chunkSize )
        {
            for ( int i = 0; i < array.Length; i += chunkSize )
            {
                int remaining = Math.Min(chunkSize, array.Length - i);
                T[] chunk = new T[ remaining ];
                Array.Copy(array, i, chunk, 0, remaining);
                yield return chunk;
            }
        }

        public static Dictionary<TValue, List<TKey>> Flip<TKey, TValue>( this Dictionary<TKey, TValue> source )
        {
            Dictionary<TValue, List<TKey>> ret = new Dictionary<TValue, List<TKey>>();

            foreach ( KeyValuePair<TKey, TValue> item in source )
            {
                if ( !ret.TryGetValue(item.Value, out _) )
                {
                    ret[ item.Value ] = new List<TKey>();
                }
                ret[ item.Value ].Add(item.Key);
            }
            return ret;
        }

        public static IEnumerable<Dictionary<TValue, List<TKey>>> Flip<TKey, TValue>( this IEnumerable<Dictionary<TKey, TValue>> source )
        {
            List<Dictionary<TValue, List<TKey>>> ret = new List<Dictionary<TValue, List<TKey>>>();
            foreach ( Dictionary<TKey, TValue> item in source )
            {
                ret.Add(item.Flip());
            }
            return ret;
        }

        public static Dictionary<TKey, TValue> Concat<TKey, TValue>( this IEnumerable<Dictionary<TKey, TValue>> source )
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
            foreach ( Dictionary<TKey, TValue> item in source )
            {
                foreach ( KeyValuePair<TKey, TValue> item1 in item )
                {
                    if ( ret.TryGetValue(item1.Key, out TValue val) )
                    {
                        if ( val is IList vallist && item1.Value is IList iList ) // TODO: instead of using IList, add support for an Ienumerable
                        {
                            foreach ( object item2 in iList )
                            {
                                vallist.Add(item2);
                            }
                        }
                        else if ( !val.Equals(item1.Value) )
                        {
                            throw new ArgumentException("Key is already present in the dictionary");
                        }
                    }
                    else
                    {
                        ret.Add(item1.Key, item1.Value);
                    }
                }
            }
            return ret;
        }
    }
}
