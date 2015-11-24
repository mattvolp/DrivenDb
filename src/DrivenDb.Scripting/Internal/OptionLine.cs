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
}