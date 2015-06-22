using System.Collections.Generic;

namespace DrivenDb.Data
{
   internal class TableMap
   {      
      public TableMap(TableDetail detail, IEnumerable<ColumnMap> columns)
      {
         Detail = detail;
         Columns = columns;
      }

      public readonly TableDetail Detail;
      public readonly IEnumerable<ColumnMap> Columns;
   }
}
