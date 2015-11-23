using System;
using System.IO;
using System.Text.RegularExpressions;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class OptionWriter      
   {
      private static readonly Regex _args = new Regex(@"\$(\d)");
      private readonly ScriptingOptions _options;
      private readonly TextWriter _writer;  

      public OptionWriter(ScriptingOptions options, TextWriter writer)
      {
         _options = options;
         _writer = writer;         
      }

      public void WriteText(string text, params object[] parameters)
      {
         var format = Reformat(text);
         var formatted = String.Format(format, parameters);

         _writer.Write(formatted);
      }

      public void WriteLine(string text, params object[] parameters)
      {
         var format = Reformat(text);
         var formatted = String.Format(format, parameters);

         _writer.WriteLine(formatted);         
      }

      public void WriteLine(string text, ScriptingOptions condition, params object[] parameters)
      {
         if ((_options & condition) == condition)
         {
            var format = Reformat(text);
            var formatted = String.Format(format, parameters);

            _writer.WriteLine(formatted);
         }
      }
      
      private static string Reformat(string input)
      {
         var escaped = input.Replace("{", @"\{").Replace("}", @"\}");
         var format = _args.Replace(escaped, "{$1}");
         var doubly = format.Replace(@"\{", "{{").Replace(@"\}", "}}");

         return doubly;
      }
   }
}
