
using System;

namespace Fastlite.DrivenDb
{
   internal sealed class DbMapper<T> : IDbMapper<T>
   {
      private readonly Action<DbRecord<T>> _action;

      public DbMapper(Action<DbRecord<T>> action)
      {
         _action = action;
      }

      public void Map(DbRecordList<T> records)
      {
         foreach (var record in records)
         {
            _action(record);
         }
      }
   }
}