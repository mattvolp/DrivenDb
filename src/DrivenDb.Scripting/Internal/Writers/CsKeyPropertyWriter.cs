using System.Linq;
using DrivenDb.Core.Extensions;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyPropertyWriter
      : ITableWriter
   {
      public TableTarget Write(TableTarget target)
      {
         return target.Chain(WriteKeyProperty);
      }

      public void WriteKeyProperty(TableTarget target)
      {
         var primaries = target.Table.GetPrimaryKeyColumns()
            .ToArray();

         if (primaries.Length > 0)
         {
            target.Writer.WriteLines(new ScriptLines()
               {
                  {"                                                             "}, // TODO: buffer spaces should be in calling class
                  {"        public $0Key PrimaryKey                              "},
                  {"        {                                                    "},
                  {"            get { return new $0Key($1); }                    "},
                  {"        }                                                    "},
               }
               , target.Table.Detail.Name
               , primaries.ScriptAsDelimitedPrivateMemberNames())
               .Ignore();
         }
      }      
   }
}
