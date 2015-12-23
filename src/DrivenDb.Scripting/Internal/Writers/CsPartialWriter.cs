using DrivenDb.Core.Extensions;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPartialWriter
      : IClassContentScripter
   {      
      public TableTarget Write(TableTarget target)
      {
         foreach (var column in target)
         {
            Write(column);
         }

         return target;
      }
      
      public void Write(ColumnTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                                                                ", ScriptingOptions.ImplementPartialPropertyChanges},
                  {"        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges},
                  {"        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges},
               }
               , target.Column.Detail.Name
               , target.Column.ScriptAsCsType())
            .Ignore();
      }      
   }
}
