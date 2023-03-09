using System.Collections.Generic;

namespace EasyZoneBuilder.Core
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Difference<T>( this IEnumerable<T> param1, IEnumerable<T> param2 )
        {
            var set2 = new HashSet<T>(param2);
            foreach ( T element in param1 )
            {
                if ( !set2.Contains(element) )
                {
                    yield return element;
                }
            }
        }
    }
}
