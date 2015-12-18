using System.Collections;
using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Interfaces
{
   internal interface ITableWriter
   {
      TableTarget Write(TableTarget target); //(ScriptTarget target, TargetWriter writer, TableMap table);
   }

   internal class TableTarget
      : IEnumerable<ColumnTarget>
   {
      public readonly ScriptTarget Target;
      public readonly TargetWriter Writer;
      public readonly TableMap Table;

      public TableTarget(ScriptTarget target, TargetWriter writer, TableMap table)
      {
         Target = target;
         Writer = writer;
         Table = table;
      }

      public IEnumerator<ColumnTarget> GetEnumerator()
      {
         foreach (var column in Table.Columns)
         {
            yield return new ColumnTarget(Target, Writer, column);
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   internal class ColumnTarget
   {
      public readonly ScriptTarget Target;
      public readonly TargetWriter Writer;
      public readonly ColumnMap Column;

      public ColumnTarget(ScriptTarget target, TargetWriter writer, ColumnMap column)
      {
         Target = target;
         Writer = writer;
         Column = column;
      }
   }
}