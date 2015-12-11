using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal
{
   internal class CsGenerator : IEntityGenerator
   {            
      private readonly ScriptTarget _target;
      private readonly ITablesWriter _writer;

      public CsGenerator(ScriptTarget target, ITablesWriter writer)
      {
         _target = target;
         _writer = writer;
      }

      public void Write(string @namespace, string contextName, TableMap[] tables)
      {         
         _writer.Write(_target, tables);         
      }      
   }
}
