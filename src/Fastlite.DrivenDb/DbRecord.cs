using System;
using System.Collections.Generic;

namespace Fastlite.DrivenDb
{
   public sealed class DbRecord<T>
   {
      private readonly T _entity;
      private readonly string[] _names;
      private readonly object[] _values;

      public DbRecord(T entity, string[] names, object[] values)
      {
         if (names == null)
            throw new ArgumentNullException("names");

         if (values == null)
            throw new ArgumentNullException("values");

         _entity = entity;
         _names = names;
         _values = values;
      }

      public T Entity
      {
         get { return _entity; }
      }

      public IReadOnlyList<string> Names
      {
         get { return _names; }
      }

      public IReadOnlyList<object> Values
      {
         get { return _values; }
      }
   }
}
