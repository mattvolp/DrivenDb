using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Core.Extensions;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyClassWriter
      : ITableWriter
   {
      public TableTarget Write(TableTarget target)
      {
         return target.Chain(WriteKeyClass);
      }

      // TODO: still room for improvement
      public void WriteKeyClass(TableTarget target)
      {
         var primaries = target.Table
            .GetPrimaryKeyColumns()// .Map(GetPrimaryKeyColumns)
            .Chain(GuardAgainsColumnOverflow)
            .ToList();
         
         if (primaries.Any())
         {
            target.Writer
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
                  , target.Table.Detail.Name
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
                  })
               .Ignore();
         }
      }

      //private static IEnumerable<ColumnMap> GetPrimaryKeyColumns(TableTarget target)
      //{
      //   foreach (var column in target)
      //   {
      //      if (column.Column.Detail.IsPrimary)
      //         yield return column.Column;
      //   }
      //}

      private static void GuardAgainsColumnOverflow(IEnumerable<ColumnMap> columns)
      {
         if (columns.Count() > 8)
            throw new Exception("Unable to script key class for tables with a primary key of more than 8 columns");
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
