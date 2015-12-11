using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptTarget      
   {            
      public ScriptTarget(ScriptingOptions options, TextWriter writer, string @namespace, string contextName)
      {
         Options = options;
         _writer = writer;
         Namespace = @namespace;
         ContextName = contextName;
      }

      // TODO: unsure about this...
      public readonly ScriptingOptions Options;
      private readonly TextWriter _writer;
      public readonly string Namespace;
      public readonly string ContextName;

      public ScriptTarget WriteTemplate<T>(IEnumerable<T> items, ScriptLines lines, ValueExtractor<T> args)
      {
         foreach (var item in items)
         {
            foreach (var scriptLine in lines)
            {
               WriteLine(scriptLine.Text, scriptLine.Options, args(item));
            }
         }

         return this;
      }

      public ScriptTarget WriteLines(ScriptLines lines, params string[] args)
      {
         foreach (var scriptLine in lines)
         {
            WriteLine(scriptLine.Text, scriptLine.Options, args);
         }

         return this;
      }

      private ScriptTarget WriteLine(string text, IReadOnlyCollection<ScriptingOptions> options, params string[] parameters)
      {
         return (options.Count == 0 || options.Any(o => (Options & o) == o))
            ? WriteLine(text, parameters)
            : this;
      }

      public ScriptTarget WriteLine(string text, params string[] parameters)
      {
         _writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
               .TrimEnd()
            );

         return this;
      }

      public override string ToString()
      {
         return _writer.ToString();
      }
   }   
}
