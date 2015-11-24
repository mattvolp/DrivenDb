using System.Collections;
using System.Collections.Generic;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
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
}