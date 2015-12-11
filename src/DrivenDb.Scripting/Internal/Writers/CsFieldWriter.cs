using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsFieldWriter
      : ITableWriter
   {
      public void Write(ScriptTarget target, TableMap table)
      {
         Write(target, table.Columns);
      }

      public void Write(ScriptTarget target, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            Write(target, column);
         }
      }

      public void Write(ScriptTarget target, ColumnMap column)
      {
         target.WriteLines(new ScriptLines()
            {
               {"                                                                "},
               {"        [DataMember]                                            ", ScriptingOptions.Serializable},
               {"        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   "},
               {"        private $3 _$0;                                         "},
            }
                           , column.Detail.Name
                           , column.Detail.IsPrimary.ScriptAsCsBoolean()
                           , column.Detail.IsGenerated.ScriptAsCsBoolean()
                           , column.ScriptAsCsType());
      }      
   }
}
