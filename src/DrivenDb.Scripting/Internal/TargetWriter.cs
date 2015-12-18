using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   public delegate string[] ValueExtractor<in T>(T item);

   internal class TargetWriter
   {
      private readonly ScriptTarget _target;

      public TargetWriter(ScriptTarget target)
      {
         _target = target;
      }

      public TargetWriter WriteTemplate<T>(IEnumerable<T> items, ScriptLines lines, ValueExtractor<T> args)
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
      
      public TargetWriter WriteLines(ScriptLines lines, params string[] args)
      {
         foreach (var scriptLine in lines)
         {
            WriteLine(scriptLine.Text, scriptLine.Options, args);
         }

         return this;
      }

      private TargetWriter WriteLine(string text, IReadOnlyCollection<ScriptingOptions> options, params string[] parameters)
      {
         return (options.Count == 0 || options.Any(o => (_target.Options & o) == o))
            ? WriteLine(text, parameters)
            : this;
      }

      private TargetWriter WriteLine(string text, params string[] parameters)
      {
         _target.Writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
               .TrimEnd()
            );

         return this;
      }

      public override string ToString()
      {
         return _target.Writer.ToString();
      }
   }
}
