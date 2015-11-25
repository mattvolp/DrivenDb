using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class OptionWriter      
   {      
      private readonly ScriptingOptions _options;
      private readonly TextWriter _writer;
      
      public OptionWriter(ScriptingOptions options, TextWriter writer)
      {
         _options = options;
         _writer = writer;         
      }

      //public void WriteSection(TableMap table, Action<TableMap> action, ScriptingOptions options)
      //{
      //   if (_options.HasFlag(options))
      //      action(table);
      //}

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
