using System.Collections.Generic;

namespace Fastlite.Framework.Collections
{
   public static class Enumerables
   {
      public static FixedList<T> ToFixedList<T>(this IReadOnlyList<T> instance)
      {
         return new FixedList<T>(instance);
      }

      public static FixedList<T> ToFixedList<T>(this IEnumerable<T> instance)
      {
         return new FixedList<T>(instance);
      }
   }
}
