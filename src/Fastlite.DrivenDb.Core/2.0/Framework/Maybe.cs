using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fastlite.DrivenDb.Core._2._0.Framework
{
   public struct Maybe<T> : IEnumerable<T>
   {
      private readonly T[] _value;

      public Maybe(T value)
      {
         _value = new [] {value};
      }

      public IEnumerator<T> GetEnumerator()
      {
         var value = _value ?? new T[0];

         return value.AsEnumerable()
            .GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
