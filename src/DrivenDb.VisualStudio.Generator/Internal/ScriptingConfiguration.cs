using System.Collections.Generic;

namespace DrivenDb.VisualStudio.Generator.Internal
{
   internal class ScriptingConfiguration
   {
      public readonly string Namespace;
      public readonly string ContextName;      
      public readonly IEnumerable<TableAlteration> Alterations;

      public ScriptingConfiguration(
           string @namespace
         , string contextName         
         , IEnumerable<TableAlteration> alterations
         )
      {
         Namespace = @namespace;
         ContextName = contextName;
         Alterations = alterations;
      }
   }
}
