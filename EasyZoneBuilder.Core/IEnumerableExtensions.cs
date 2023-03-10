using System;
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
    }
}
