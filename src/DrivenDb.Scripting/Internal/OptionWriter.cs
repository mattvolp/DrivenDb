using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class OptionLine
   {
      public OptionLine(string line, params ScriptingOptions[] options) 
      {         
         Line = line;
         Options = options;
      }

      public readonly ScriptingOptions[] Options;
      public readonly string Line;      
   }

   internal class OptionLines 
      : IEnumerable<OptionLine>
   {      
      private readonly List<OptionLine> _lines = new List<OptionLine>();
      
      public void Add(string line) 
      {
         _lines.Add(new OptionLine(line));
      }

      public void Add(string line, params ScriptingOptions[] options)
      {         
         _lines.Add(new OptionLine(line, options)); 
      }

      public static implicit operator OptionLines(OptionLine[] lines)
      {
         return null;
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
      private bool _write = true;

      public OptionWriter(ScriptingOptions options, TextWriter writer)
      {
         _options = options;
         _writer = writer;         
      }

      public OptionWriter WriteIf(bool condition)
      {
         _write = condition;
         return this;
      }
      
      public OptionWriter WriteTemplate<T>(IEnumerable<T> items, OptionLines lines, Func<T, object[]> args)
      {
         foreach (var item in items)
         {
            foreach (var scriptLine in lines)
            {
               if (WriteLine(scriptLine.Line, scriptLine.Options, args(item)))
                  break;
            }
         }
         return this;
      }
      
      public OptionWriter WriteLines(OptionLines lines, params object[] args)
      {         
         foreach (var scriptLine in lines)
         {
            if (WriteLine(scriptLine.Line, scriptLine.Options, args))
               break;
         }

         return this;
      }

      //public OptionWriter WriteText(string text, params object[] parameters)
      //{
      //   _writer.Write(
      //      text.FromDollarToStringFormat()
      //         .Format(parameters)
      //      );

      //   return this;
      //}

      public OptionWriter WriteLine(string text, params object[] parameters)
      {
         _writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );

         return this;
      }

      public OptionWriter WriteLine(string text, ScriptingOptions options, params object[] parameters)
      {
         _writer.WriteLine(
            text.FromDollarToStringFormat()
               .Format(parameters)
            );

         return this;
      }

      private bool WriteLine(string text, ScriptingOptions[] options, params object[] parameters)
      {
         foreach (var option in options)
         {            
            if ((_options & option) == option)
            {
               _writer.WriteLine(
                  text.FromDollarToStringFormat()
                     .Format(parameters)
                  );
            }

            return true;
         }

         return false;
      }      
   }
}
