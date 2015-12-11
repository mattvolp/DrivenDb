using System.Linq;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyClassWriter
      : ITableWriter
   {
      public void Write(ScriptTarget target, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         primaries.GuardAgainstKeyClassOverflow();

         if (primaries.Any())
         {
            target
               .WriteLines(new ScriptLines()
                  {
                     {"                                                          "},
                     {"    public class $0Key                                    "},
                     {"        : Tuple<$1>                                       "},
                     {"    {                                                     "},
                     {"        public $0Key($2)                                  "},
                     {"            : base($3)                                    "},
                     {"        {                                                 "},
                  }
                  , table.Detail.Name
                  , primaries.ScriptAsDelimitedCsTypes()
                  , primaries.ScriptAsDelimitedCsTypedParameterNames()
                  , primaries.ScriptAsDelimitedParameterNames())

               .WriteTemplate(primaries, new ScriptLines()
                  {
                     {"            $0 = $1;                                     "},
                  }, ColumnExtractor1)

               .WriteLines(new ScriptLines()
                  {
                     {"        }                                                 "},
                     {"                                                          "},
                  })

               .WriteTemplate(primaries, new ScriptLines()
                  {
                     {"        public readonly $0 $1;                            "},
                  }, ColumnExtractor2)

               .WriteLines(new ScriptLines()
                  {
                     {"   }                                                      "},
                  });
         }
      }

      private static string[] ColumnExtractor1(ColumnMap column)
      {
         return new[]
            {
               column.Detail.Name,
               column.ScriptAsParameterName()
            };
      }

      private static string[] ColumnExtractor2(ColumnMap column)
      {
         return new[]
            {
               column.ScriptAsCsType(),
               column.Detail.Name
            };
      }
   }
}
