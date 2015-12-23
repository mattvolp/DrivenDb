using DrivenDb.Core.Extensions;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsConstructorWriter      
   {
      private readonly CsDefaultTranslator _defaults;

      public CsConstructorWriter(CsDefaultTranslator defaults)
      {
         _defaults = defaults;
      }

      public Script<TableMap> Write(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<TableMap>(tm, so, sc
            .Append(new ScriptSegment(tm.Detail.Name)
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               })
            .AppendForEach(tm.GetColumnsWithDefaultDefinitions(), ColumnExtractor, new ScriptSegment()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
                  // TODO: comma not or?
                  {"            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking},
               })
            .Append(new ScriptSegment()
               {
                  {"        }                                                       "},
                  {"                                                                ", ScriptingOptions.ImplementStateTracking},
                  {"        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking},
                  {"        {                                                       ", ScriptingOptions.ImplementStateTracking},
                  {"            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking},
                  {"        }                                                       ", ScriptingOptions.ImplementStateTracking},
               }));
      }

      private string[] ColumnExtractor(ColumnMap column)
      {
         return new[]
            {
               column.Detail.Name,
               column.ScriptAsCsType(),
               _defaults.Translate(column)
               //MsSqlScriptingServices.ToCsScriptedDefaultValue(target.Options, d.Detail)  // TODO:
            };
      }
   }
}
