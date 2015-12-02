using System.Collections;
using System.Collections.Generic;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptLines 
      : IEnumerable<ScriptLine>
   {      
      private readonly List<ScriptLine> _lines = new List<ScriptLine>();
      
      public void Add(string line) 
      {
         _lines.Add(new ScriptLine(line));
      }
      
      public void Add(string line, params ScriptingOptions[] options)
      {         
         _lines.Add(new ScriptLine(line, options)); 
      }
      
      public IEnumerator<ScriptLine> GetEnumerator()
      {
         return _lines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}