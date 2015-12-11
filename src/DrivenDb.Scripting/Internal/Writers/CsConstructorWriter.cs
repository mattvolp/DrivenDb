using System.Linq;
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

      public void Write(ScriptTarget target, TableMap table)
      {
         var defaults = table.GetColumnsWithDefaultDefinitions()
            .ToArray();

         target
            .WriteLines(new ScriptLines()
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               }, table.Detail.Name)

            .WriteTemplate(defaults, new ScriptLines()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
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
               });
      }

      private string[] ColumnExtractor(ColumnMap column)
      {
         return new[]
            {
               column.Detail.Name,
            column.ScriptAsCsType(),
            _defaults.Translate(column)
            //MsSqlScriptingServices.ToCsScriptedDefaultValue(target.Options, d.Detail) 
            };
      }
   }
}
