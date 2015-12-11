using System.Collections;
using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Collections
{
   internal class TableWriterCollection
      : IEnumerable<ITableWriter>
         , ITableWriter
   {
      private readonly IReadOnlyCollection<ITableWriter> _writers;

      public TableWriterCollection(params ITableWriter[] writers)
      {
         _writers = writers;
      }

      public void Write(ScriptTarget target, TableMap table)
      {
         foreach (var writer in _writers)
         {
            writer.Write(target, table);
         }
      }

      public IEnumerator<ITableWriter> GetEnumerator()
      {
         return _writers.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }      
   }
}