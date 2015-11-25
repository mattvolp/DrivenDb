using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class OptionWriter      
   {      
      public readonly ScriptingOptions _options;
      public readonly TextWriter _writer;
      public readonly bool _penUp;

      public OptionWriter(ScriptingOptions options, TextWriter writer, bool penUp)
      {
         _options = options;
         _writer = writer;
         _penUp = penUp;
      }
      
      public OptionWriter WriteTemplate<T>(IEnumerable<T> items, OptionLines lines, Func<T, string[]> args)
      {
         foreach (var item in items)
         {
            foreach (var scriptLine in lines)
            {
               WriteLine(scriptLine.Line, scriptLine.Options, args(item));
            }
         }

         return this;
      }
      
      public OptionWriter WriteLines(OptionLines lines, params string[] args)
      {         
         foreach (var scriptLine in lines)
         {
            WriteLine(scriptLine.Line, scriptLine.Options, args);               
         }

         return this;
      }
      
      public OptionWriter WriteLine(string text, params string[] parameters)
      {
         _writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );

         return this;
      }

      public OptionWriter WriteLine(string text, ScriptingOptions options, params string[] parameters)
      {
         WriteLine(text, new[] {options}, parameters);

         return this;
      }

      private void WriteLine(string text, ScriptingOptions[] options, params string[] parameters)
      {
         if (options.Length == 0 || options.Any(o => (_options & o) == o))
         {
            _writer.WriteLine(
               text.FromDollarToStringFormat()
                  .Format(parameters)
                  .TrimEnd()
               );            
         }         
      }      
   }
}
