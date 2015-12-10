using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsConstructorWriter
   {
      public void Write(ScriptTarget target, TableMap table)
      {
         var defaults = table.GetColumnsWithDefaultDefinitions()
            .ToArray();

         target
            .WriteLinesAndContinue(new ScriptLines()
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               }, table.Detail.Name)

            .WriteTemplateAndContinue(defaults, new ScriptLines()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
                  {"            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking},
               }, d => new[]
                  {
                       d.Detail.Name
                     , d.ScriptAsCsType()
                     , MsSqlScriptingServices.ToCsScriptedDefaultValue(target.Options, d.Detail) //d.ScriptAsDefaultValue(target.Options)
                  })

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
   }
}
