using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class WritingCompositions
   {
      //
      // WRITE LINES
      //

      public static ScriptTarget WriteLineAndContinue(this ScriptTarget target, string text, params string[] parameters)
      {
         target.WriteLine(text, parameters);
         return target;
      }

      public static ScriptTarget WriteLinesAndContinue(this ScriptTarget target, ScriptLines lines, params string[] args)
      {
         target.WriteLines(lines, args);
         return target;
      }

      //
      // WRITE TEMPLATES
      //

      public static ScriptTarget WriteTemplateAndContinue<T>(this ScriptTarget target, IEnumerable<T> items, ScriptLines lines, WritingServices.ValueExtractor<T> args)
      {
         target.WriteTemplate(items, lines, args);
         return target;
      }      
   }
}
