using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsClassScripter
      : IScripter<NamespaceDetail>
   {
      private readonly IScripter<TableMap> _content;

      public CsClassScripter(IScripter<TableMap> content)
      {
         _content = content;
      }

      public Script<NamespaceDetail> Script(NamespaceDetail tm, ScriptingOptions so, SegmentCollection sc)
      {
         throw new System.NotImplementedException();
      }

      private Script<TableMap> Script(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<TableMap>(tm, so, sc)            
            .Bind(WriteClassOpen)
            .Bind(_content)            
            .Bind(WriteClassClose);
      }
      
      private static Script<TableMap> WriteClassOpen(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<TableMap>(tm, so,
            sc.Append(new ScriptSegment(
                 tm.Detail.Schema
               , tm.Detail.Name
               , so.ScriptAsDelimitedImpliedInterfaces() // TODO: unsure about this
               )
               {
                  {"                                                                   "},
                  {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
                  {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
                  {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
                  {"    public partial class $1 $2                                     "},
                  {"    {                                                              "},
                  {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking}, //TODO: comma not or?
                  {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
               }));
      }
      
      private static Script<TableMap> WriteClassClose(TableMap tm, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<TableMap>(tm, so,
            sc.Append(new ScriptSegment()
               {
                  {"    }"}
               }));
      }      
   }
}
