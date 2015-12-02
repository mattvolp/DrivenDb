using System.IO;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptTarget      
   {      
      public readonly ScriptingOptions Options;
      public readonly TextWriter Writer;
      
      public ScriptTarget(ScriptingOptions options, TextWriter writer)
      {
         Options = options;
         Writer = writer;      
      }      
   }   
}
