using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptLine
   {
      public ScriptLine(string text, params ScriptingOptions[] options) 
      {         
         Text = text;
         Options = options;
      }

      public readonly ScriptingOptions[] Options;
      public readonly string Text;      
   }   
}