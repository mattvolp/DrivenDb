using System.Collections;
using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Interfaces
{   
   internal interface ITablesWriter
   {
      //void Write(ScriptTarget target, TargetWriter writer, IReadOnlyCollection<TableMap> tables);
      TablesTarget Write(TablesTarget target);
   }

   internal class TablesTarget
      : IEnumerable<TableTarget>
   {
      public readonly ScriptTarget Target;
      public readonly TargetWriter Writer;
      public readonly IReadOnlyCollection<TableMap> Tables;

      public TablesTarget(
           ScriptTarget target
         , TargetWriter writer
         , IReadOnlyCollection<TableMap> tables
         )
      {
         Target = target;
         Writer = writer;
         Tables = tables;
      }

      public IEnumerator<TableTarget> GetEnumerator()
      {
         foreach (var table in Tables)
         {
            yield return new TableTarget(Target, Writer, table);
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
