using System.Collections;
using System.Collections.Generic;

namespace Fastlite.DrivenDb
{
   public sealed class DbRecordList<T> : IReadOnlyList<DbRecord<T>>
   {
      private readonly IReadOnlyList<DbRecord<T>> _records;

      public DbRecordList(IReadOnlyList<DbRecord<T>> records)
      {
         _records = records;
      }

      public DbRecord<T> this[int index]
      {
         get { return _records[index]; }
      }

      public int Count
      {
         get { return _records.Count; }
      }

      public IEnumerator<DbRecord<T>> GetEnumerator()
      {
         return _records.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }      
   }
}
