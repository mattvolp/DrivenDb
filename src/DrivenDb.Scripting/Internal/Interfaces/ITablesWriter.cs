using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Interfaces
{
   internal interface ITablesWriter
   {
      void Write(ScriptTarget target, IReadOnlyCollection<TableMap> tables);
   }
}
