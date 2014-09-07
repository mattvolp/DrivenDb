using System;
using System.Collections;
using System.Collections.Generic;

namespace Fastlite.DrivenDb
{
   public sealed class DbResultList<T> : IReadOnlyList<T>
   {
      private readonly IReadOnlyList<T> _results;

      public DbResultList(IReadOnlyList<T> results)
      {
         if (results == null)
            throw new ArgumentNullException("results");

         _results = results;
      }

      public T this[int index]
      {
         get { return _results[index]; }
      }

      public int Count
      {
         get { return _results.Count; }
      }

      public IEnumerator<T> GetEnumerator()
      {         
         return _results.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
