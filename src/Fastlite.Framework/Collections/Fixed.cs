using System;
using System.Collections;
using System.Collections.Generic;

namespace Fastlite.Framework.Collections
{
   public struct Fixed<T> : IReadOnlyList<T>
   {
      private readonly IReadOnlyList<T> _list;

      public Fixed(IReadOnlyList<T> list)
      {
         _list = list;
      }

      public Fixed(IEnumerable<T> enumerable)
      {
         _list = new List<T>(enumerable);
      }

      public T this[int index]
      {
         get
         {
            if (_list != null)
            {
               return _list[index];
            }

            throw new ArgumentOutOfRangeException("index");
         }
      }

      public int Count
      {
         get
         {
            return _list != null 
               ? _list.Count 
               : 0;
         }
      }

      public IEnumerator<T> GetEnumerator()
      {
         return (_list ?? new List<T>())
            .GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
