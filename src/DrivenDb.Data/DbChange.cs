using System.Collections.Generic;
using System.Runtime.Serialization;
using DrivenDb.Core;

namespace DrivenDb.Data
{
   // TODO: test, remove properties, test serialization
   [DataContract]
   public class DbChange
   {
      public DbChange(
           DbChangeType changeType
         , string affectedTable
         , IEnumerable<string> affectedColumns
         , IDbEntity entity
         )
      {
         ChangeType = changeType;
         AffectedTable = affectedTable;
         AffectedColumns = affectedColumns;
         Entity = entity;
      }

      [DataMember]
      public DbChangeType ChangeType
      {
         get;
         private set;
      }

      [DataMember]
      public string AffectedTable
      {
         get;
         private set;
      }

      [DataMember]
      public IEnumerable<string> AffectedColumns
      {
         get;
         private set;
      }

      [DataMember]
      public IDbEntity Entity
      {
         get;
         private set;
      }
   }
}
