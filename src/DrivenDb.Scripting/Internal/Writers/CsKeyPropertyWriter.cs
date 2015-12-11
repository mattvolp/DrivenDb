using System.Linq;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyPropertyWriter
      : ITableWriter
   {
      public void Write(ScriptTarget target, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         if (primaries.Length > 0)
         {
            target.WriteLines(new ScriptLines()
               {
                  {"                                                             "},
                  {"        public $0Key PrimaryKey                              "},
                  {"        {                                                    "},
                  {"            get { return new $0Key($1); }                    "},
                  {"        }                                                    "},
               }
               , table.Detail.Name
               , primaries.ScriptAsDelimitedPrivateMemberNames());
         }
      }
   }
}
