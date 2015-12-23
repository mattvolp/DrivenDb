using System.Collections;
using System.Collections.Generic;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Collections
{
   internal class TableWriterCollection
      : IEnumerable<IClassContentScripter>
      , IClassContentScripter
   {
      private readonly IReadOnlyCollection<IClassContentScripter> _writers;

      public TableWriterCollection(params IClassContentScripter[] writers)
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

      public IEnumerator<IClassContentScripter> GetEnumerator()
      {
         return _writers.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}