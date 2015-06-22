using System.Collections.Generic;

namespace DrivenDb.Core
{
   public interface IDbEntity
   {
      IEnumerable<DbEntityChange> Changes
      {
         get;
      }

      EntityState State
      {
         get;
      }

      void Change(string columnName, object value);
      void Delete();
      void Reset();
   }
}