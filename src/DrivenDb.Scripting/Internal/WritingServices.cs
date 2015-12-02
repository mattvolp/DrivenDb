using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class WritingServices
   {
      internal delegate string[] ValueExtractor<in T>(T item);
      
      public static void WriteTemplate<T>(this ScriptTarget target, IEnumerable<T> items, ScriptLines lines, ValueExtractor<T> args)
      {
         foreach (var item in items)
         {
            foreach (var scriptLine in lines)
            {
               WriteLine(target, scriptLine.Line, scriptLine.Options, args(item));
            }
         }
      }
      
      public static void WriteLines(this ScriptTarget target, ScriptLines lines, params string[] args)
      {
         foreach (var scriptLine in lines)
         {
            WriteLine(target, scriptLine.Line, scriptLine.Options, args);
         }
      }

      public static void WriteLines(this ScriptTarget target, ScriptLines lines)
      {
         foreach (var scriptLine in lines)
         {
            WriteLine(target, scriptLine.Line, scriptLine.Options);
         }
      }
      
      public static void WriteLine(this ScriptTarget target, string text, params string[] parameters)
      {
         target.Writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );
      }

      public static void WriteLine(this ScriptTarget target, string text, ScriptingOptions options, params string[] parameters)
      {
         WriteLine(target, text, new[] { options }, parameters);
      }

      private static void WriteLine(this ScriptTarget target, string text, ScriptingOptions[] options, params string[] parameters)
      {
         if (options.Length == 0 || options.Any(o => (target.Options & o) == o))
         {
            target.WriteLine(
               text.FromDollarToStringFormat()
                  .Format(parameters)
                  .TrimEnd()
               );
         }
      }
   }
}
