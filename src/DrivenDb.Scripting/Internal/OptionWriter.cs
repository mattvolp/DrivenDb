using System.Collections;
using System.Collections.Generic;
using System.IO;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   public class OptionLine
   {
      public OptionLine(string line, object[] args)
      {
         Line = line;
         Args = args;
      }

      public readonly string Line;
      public readonly object[] Args;
   }

   public class OptionLines 
      : IEnumerable<OptionLine>
   {
      private readonly List<OptionLine> _lines = new List<OptionLine>();
      
      public void Add(string line, params object[] args)
      {
         _lines.Add(new OptionLine(line, args));
      }
      
      public IEnumerator<OptionLine> GetEnumerator()
      {
         return _lines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   internal class OptionWriter      
   {      
      private readonly ScriptingOptions _options;
      private readonly TextWriter _writer;  

      public OptionWriter(ScriptingOptions options, TextWriter writer)
      {
         _options = options;
         _writer = writer;         
      }
      
      public OptionWriter WriteLines(OptionLines lines)
      {
         //_writer.Write(
         //   text.FromDollarToStringFormat()
         //      .Format(parameters)
         //   );

         foreach (var scriptLine in lines)
         {
            WriteLine(scriptLine.Line, scriptLine.Args);
         }

         return this;
      }

      public OptionWriter WriteText(string text, params object[] parameters)
      {
         _writer.Write(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );

         return this;
      }

      public OptionWriter WriteLine(string text, params object[] parameters)
      {
         _writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );

         return this;
      }

      public OptionWriter WriteLine(string text, ScriptingOptions condition, params object[] parameters)
      {
         if ((_options & condition) == condition)
         {
            WriteLine(text, parameters);            
         }

         return this;
      }      
   }
}
