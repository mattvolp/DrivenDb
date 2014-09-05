using System.Collections.Generic;

namespace Fastlite.Framework.Collections
{
   public static class Enumerables
   {
      public static Fixed<T> ToFixedList<T>(this IReadOnlyList<T> instance)
      {
         return new Fixed<T>(instance);
      }

      public static Fixed<T> ToFixedList<T>(this IEnumerable<T> instance)
      {
         return new Fixed<T>(instance);
      }
   }
}
