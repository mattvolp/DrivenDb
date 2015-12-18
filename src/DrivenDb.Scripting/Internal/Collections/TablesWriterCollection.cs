using System.Collections;
using System.Collections.Generic;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Collections
{
   internal class TablesWriterCollection
      : IEnumerable<ITablesWriter>
      , ITablesWriter
   {
      private readonly IReadOnlyCollection<ITablesWriter> _writers;

      public TablesWriterCollection(params ITablesWriter[] writers)
      {
         _writers = writers;
      }

      public TablesTarget Write(TablesTarget target)
      {
         foreach (var writer in _writers)
         {
            writer.Write(target);
         }

         return target;
      }
      
      public IEnumerator<ITablesWriter> GetEnumerator()
      {
         return _writers.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }      
   }
}
