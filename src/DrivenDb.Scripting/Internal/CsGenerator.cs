using System.IO;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Writers;

namespace DrivenDb.Scripting.Internal
{
   internal class CsGenerator : IEntityGenerator
   {
      private readonly ScriptingOptions _options;
      private readonly CsNamespaceWriter _scripter;
      private readonly ScriptTarget _target;

      public CsGenerator(ScriptingOptions options, TextWriter writer, CsNamespaceWriter scripter)
      {
         _options = options;
         _scripter = scripter;
         _target = new ScriptTarget(options, writer);
      }

      public void Write(string @namespace, string contextName, TableMap[] tables)
      {
         _scripter.Write(_target, @namespace, contextName, tables);
         //CsUnitScriptingServices.WriteUnit(_target, @namespace, contextName, tables);         
      }      
   }
}
