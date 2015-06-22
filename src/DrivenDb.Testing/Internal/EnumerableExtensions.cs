using System;
using System.Collections.Generic;

namespace DrivenDb.Testing.Internal
{
   internal static class EnumerableExtensions
   {
      public static IEnumerable<T> DistinctBy<T, V>(this IEnumerable<T> enumerable, Func<T, V> predicate)
      {
         var hashset = new HashSet<V>();

         foreach (var item in enumerable)
         {
            var key = predicate(item);

            if (hashset.Add(key))
            {
               yield return item;
            }
         }
      }
   }
}
