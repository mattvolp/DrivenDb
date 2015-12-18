using System.IO;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptTarget      
   {            
      public ScriptTarget(ScriptingOptions options, TextWriter writer, string @namespace, string contextName)
      {
         Options = options;
         Writer = writer;
         Namespace = @namespace;
         ContextName = contextName;
      }
      
      public readonly ScriptingOptions Options;
      public readonly TextWriter Writer;
      public readonly string Namespace;
      public readonly string ContextName;      
   }   
}
