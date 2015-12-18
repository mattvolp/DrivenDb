using System.Collections;
using System.Collections.Generic;
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

      public TableTarget Write(TableTarget target)
      {
         foreach (var writer in _writers)
         {
            writer.Write(target);
         }

         return target;
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