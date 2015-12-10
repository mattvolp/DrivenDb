using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsContextWriter
   {
      public void Write(ScriptTarget target, string contextName, IEnumerable<TableDetail> tables)
      {
         target
            .WriteLinesAndContinue(new ScriptLines()
               {
                  {"                                                                                              "},
                  {"    public class $0 : DataContext                                                             "},
                  {"    {                                                                                         "},
                  {"        private static readonly MappingSource _mappingSource = new AttributeMappingSource();  "},
                  {"                                                                                              "},
                  {"        public $0() : base(\"_\", _mappingSource)                                             "},
                  {"        {                                                                                     "},
                  {"        }                                                                                     "},
               }, contextName)

            .WriteTemplateAndContinue(tables, new ScriptLines()
               {
                  {"                                                                                              "},
                  {"        public Table<$0> $0                                                                   "},
                  {"        {                                                                                     "},
                  {"            get { return this.GetTable<$0>(); }                                               "},
                  {"        }                                                                                     "},
               }, t => new[] { t.Name })

            .WriteLines(new ScriptLines()
               {
                  {"    }                                                                                         "},
               });
      }
   }
}
