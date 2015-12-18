using DrivenDb.Core.Extensions;
using DrivenDb.Data;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsContextWriter
      : ITablesWriter
   {
      public TablesTarget Write(TablesTarget target)
      {
         return target.Chain(WriteContext);
      }

      public void WriteContext(TablesTarget target)
      {
         target.Writer
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
               }, target.Target.ContextName)

            .WriteTemplate(target.Tables, new ScriptLines()
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
               })
            .Ignore();
      }

      private static string[] TemplateExtractor(TableMap table)
      {
         return new[] {table.Detail.Name};
      }      
   }
}
