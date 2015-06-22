using System;
using System.Collections;
using System.Collections.Generic;

namespace DrivenDb.Data
{
   // TODO: test
   public struct DbResultSet<T>
      : IEnumerable<T>
   {
      private readonly IEnumerable<T> _enumerable;

      public DbResultSet(IEnumerable<T> enumerable)
      {
         if (enumerable == null) throw new ArgumentNullException("enumerable");

         _enumerable = enumerable;
      }

      public IEnumerator<T> GetEnumerator()
      {
         return _enumerable.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}