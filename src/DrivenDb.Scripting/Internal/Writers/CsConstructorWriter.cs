using DrivenDb.Core.Extensions;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsConstructorWriter
      : ITableWriter
   {
      private readonly CsDefaultTranslator _defaults;

      public CsConstructorWriter(CsDefaultTranslator defaults)
      {
         _defaults = defaults;
      }

      public TableTarget Write(TableTarget target)
      {
         return target.Chain(WriteContructor);
      }

      public void WriteContructor(TableTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               }, target.Table.Detail.Name)

            .WriteTemplate(target.Table.GetColumnsWithDefaultDefinitions(), new ScriptLines()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
                  // TODO: comma not or?
                  {"            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking},
               }, ColumnExtractor)

            .WriteLines(new ScriptLines()
               {
                  {"        }                                                       "},
                  {"                                                                ", ScriptingOptions.ImplementStateTracking},
                  {"        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking},
                  {"        {                                                       ", ScriptingOptions.ImplementStateTracking},
                  {"            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking},
                  {"        }                                                       ", ScriptingOptions.ImplementStateTracking},
               })
            .Ignore();
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
