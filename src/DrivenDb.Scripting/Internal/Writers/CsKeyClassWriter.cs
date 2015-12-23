using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyClassWriter
      : IScripter<NamespaceDetail>
   {
      public Script<NamespaceDetail> Script(NamespaceDetail nd, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<NamespaceDetail>(nd, so, 
            new SegmentCollection(nd.Tables.SelectMany(t => Write(t, so, sc))));
      }

      private IEnumerable<ScriptSegment> Write(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<TableMap>(tm, so, sc)
            .Bind(GuardAgainsColumnOverflow)
            .BindIf(AnyKeyColumns, WriteKeyClass)
            .Segments;
      }
      
      private static Script<TableMap> GuardAgainsColumnOverflow(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         if (tm.GetPrimaryKeyColumns().Count() > 8)
            throw new Exception("Unable to script key class for tables with a primary key of more than 8 columns");

         return new Script<TableMap>(tm, so, sc);
      }

      private static bool AnyKeyColumns(TableMap tm, ScriptingOptions scriptingOptions, SegmentCollection arg3)
      {
         return tm.GetPrimaryKeyColumns()
            .Any();
      }
      
      private static Script<TableMap> WriteKeyClass(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         var primaries = tm.GetPrimaryKeyColumns()
            .ToArray();

         return new Script<TableMap>(tm, so, sc
            .Append(new ScriptSegment(
                 tm.Detail.Name
               , primaries.ScriptAsDelimitedCsTypes()   // shit-tay
               , primaries.ScriptAsDelimitedCsTypedParameterNames()
               , primaries.ScriptAsDelimitedParameterNames())
               {
                  {"                                                          "},
                  {"    public class $0Key                                    "},
                  {"        : Tuple<$1>                                       "},
                  {"    {                                                     "},
                  {"        public $0Key($2)                                  "},
                  {"            : base($3)                                    "},
                  {"        {                                                 "},
               })
            .AppendForEach(primaries, ValueExtractor1, new ScriptSegment()
               {
                  {"            $0 = $1;                                      "},
               })
            .Append(new ScriptSegment()
               {
                  {"        }                                                 "},
                  {"                                                          "},
               })
            .AppendForEach(primaries, ValueExtractor2, new ScriptSegment()
               {
                  {"        public readonly $0 $1;                            "},
               })
            .Append(new ScriptSegment()
               {
                  {"   }                                                      "},
               }));
      }
      
      private static string[] ValueExtractor1(ColumnMap column)
      {
         return new[]
            {
               column.Detail.Name,
               column.ScriptAsParameterName()
            };
      }

      private static string[] ValueExtractor2(ColumnMap column)
      {
         return new[]
            {
               column.ScriptAsCsType(),
               column.Detail.Name
            };
      }      
   }
}
