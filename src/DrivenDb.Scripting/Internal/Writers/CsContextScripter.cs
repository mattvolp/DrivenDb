using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsContextScripter   
      : IScripter<NamespaceDetail>
   {      
      public Script<NamespaceDetail> Script(NamespaceDetail nd, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<NamespaceDetail>(nd, so, sc
            .Append(new ScriptSegment(nd.Context)
               {
                  {"                                                                                              "},
                  {"    public class $0 : DataContext                                                             "},
                  {"    {                                                                                         "},
                  {"        private static readonly MappingSource _mappingSource = new AttributeMappingSource();  "},
                  {"                                                                                              "},
                  {"        public $0() : base(\"_\", _mappingSource)                                             "},
                  {"        {                                                                                     "},
                  {"        }                                                                                     "},
               })
            .AppendForEach(nd.Tables, TemplateExtractor, new ScriptSegment()
               {
                  {"                                                                                              "},
                  {"        public Table<$0> $0                                                                   "},
                  {"        {                                                                                     "},
                  {"            get { return this.GetTable<$0>(); }                                               "},
                  {"        }                                                                                     "},
               })
            .Append(new ScriptSegment()
               {
                  {"    }                                                                                         "},
               }));
      }

      private static string[] TemplateExtractor(TableMap table)
      {
         return new[] { table.Detail.Name };
      }      
   }
}
