using System;

namespace Fastlite.DrivenDb.Data._2._0
{
   public sealed class DbRecord2<T>
   {      
      private readonly string[] _names;
      private readonly object[] _values;

      public DbRecord2(string[] names, object[] values)
      {
         if (names == null)
            throw new ArgumentNullException("names");

         if (values == null)
            throw new ArgumentNullException("values");

         _names = names;
         _values = values;
      }

      public T Entity
      {
         get; set;
      }
   }
}
