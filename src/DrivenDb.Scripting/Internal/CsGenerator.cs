using System.IO;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class CsGenerator : IEntityGenerator
   {
      private readonly ScriptingOptions _options;
      private readonly ScriptTarget _target;

      public CsGenerator(ScriptingOptions options, TextWriter writer)
      {
         _options = options;
         _target = new ScriptTarget(options, writer);
      }

      public void Write(string @namespace, string contextName, TableMap[] tables)
      {         
         CsScriptingServices.ScriptUnit(_target, @namespace, contextName, tables);         
      }      
   }
}
