using DrivenDb.Core.Extensions;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsFieldWriter
      : IClassContentScripter
   {
      public TableTarget Write(TableTarget target)
      {
         foreach (var column in target)
         {
            WriteField(column);
         }

         return target;
      }
      
      public void WriteField(ColumnTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                                                                "},
                  {"        [DataMember]                                            ", ScriptingOptions.Serializable},
                  {"        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   "},
                  {"        private $3 _$0;                                         "},
               }
               , target.Column.Detail.Name
               , target.Column.Detail.IsPrimary.ScriptAsCsBoolean()
               , target.Column.Detail.IsGenerated.ScriptAsCsBoolean()
               , target.Column.ScriptAsCsType())
            .Ignore();
      }      
   }
}
