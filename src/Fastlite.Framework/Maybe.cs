using System;
using System.Collections;
using System.Collections.Generic;

namespace Fastlite.Framework
{
   public struct Maybe<T> : IReadOnlyCollection<T>
   {
      private readonly bool _isSet;
      private readonly T _value;

      public Maybe(T value)
      {
         _value = value;
         _isSet = true;
      }

      public bool HasValue
      {
         get { return _isSet; }
      }

      public T Value
      {
         get
         {
            if (!_isSet) 
               throw new InvalidOperationException("Value not set");
            
            return _value;
         }
      }

      int IReadOnlyCollection<T>.Count
      {
         get { return _isSet ? 1 : 0; }
      }

      public IEnumerator<T> GetEnumerator()
      {
         return _isSet
            ? (new List<T>() {_value}).GetEnumerator()
            : (new List<T>()).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
