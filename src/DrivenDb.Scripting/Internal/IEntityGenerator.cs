using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal
{
   internal interface IEntityGenerator
   {
      void Write(string @namespace, string contextName, IEnumerable<TableMap> tables);
   }
}