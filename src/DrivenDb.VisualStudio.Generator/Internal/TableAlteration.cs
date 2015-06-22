using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DrivenDb.VisualStudio.Generator.Internal
{
   [DataContract]
   internal class TableAlteration
   {      
      public TableAlteration(
           string tableName
         , IEnumerable<ColumnAlteration> columns
         )
      {
         TableName = tableName;
         Columns = columns;
      }

      [DataMember]
      public readonly string TableName;

      [DataMember]
      public readonly IEnumerable<ColumnAlteration> Columns;
   }
}
