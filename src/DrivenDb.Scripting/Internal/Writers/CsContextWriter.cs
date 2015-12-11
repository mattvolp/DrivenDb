using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsContextWriter
      : ITablesWriter
   {
      public void Write(ScriptTarget target, IReadOnlyCollection<TableMap> tables)
      {
         target
            .WriteLines(new ScriptLines()
               {
                  {"                                                                                              "},
                  {"    public class $0 : DataContext                                                             "},
                  {"    {                                                                                         "},
                  {"        private static readonly MappingSource _mappingSource = new AttributeMappingSource();  "},
                  {"                                                                                              "},
                  {"        public $0() : base(\"_\", _mappingSource)                                             "},
                  {"        {                                                                                     "},
                  {"        }                                                                                     "},
               }, target.ContextName)

            .WriteTemplate(tables, new ScriptLines()
               {
                  {"                                                                                              "},
                  {"        public Table<$0> $0                                                                   "},
                  {"        {                                                                                     "},
                  {"            get { return this.GetTable<$0>(); }                                               "},
                  {"        }                                                                                     "},
               }, TemplateExtractor)

            .WriteLines(new ScriptLines()
               {
                  {"    }                                                                                         "},
               });
      }

      private static string[] TemplateExtractor(TableMap table)
      {
         return new [] {table.Detail.Name};
      }
   }
}
