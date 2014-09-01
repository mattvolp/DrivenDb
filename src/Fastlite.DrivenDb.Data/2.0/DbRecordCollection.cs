using System.Collections;
using System.Collections.Generic;

namespace Fastlite.DrivenDb.Data._2._0
{
   public sealed class DbRecordCollection<T> : IReadOnlyList<DbRecord2<T>>
   {
      private readonly IReadOnlyList<DbRecord2<T>> _records;

      public DbRecordCollection(IReadOnlyList<DbRecord2<T>> records)
      {
         _records = records;
      }

      public DbRecord2<T> this[int index]
      {
         get { return _records[index]; }
      }

      public int Count
      {
         get { return _records.Count; }
      }

      public IEnumerator<DbRecord2<T>> GetEnumerator()
      {
         return _records.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }      
   }
}
