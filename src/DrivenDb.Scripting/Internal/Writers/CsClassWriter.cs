using DrivenDb.Core.Extensions;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsClassWriter
      : ITablesWriter
   {
      private readonly ITableWriter _content;
      
      public CsClassWriter(ITableWriter content)
      {
         _content = content;         
      }

      public TablesTarget Write(TablesTarget target)
      {
         foreach (var table in target)
         {
            Write(table).Ignore();
         }

         return target;
      }

      public TableTarget Write(TableTarget target)
      {
         return target
            .Chain(OpenClass)
            .Hitch(_content.Write)
            .Chain(CloseClass);
      }
      
      private static void OpenClass(TableTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                                                                   "},
                  {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
                  {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
                  {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
                  {"    public partial class $1 $2                                     "},
                  {"    {                                                              "},
                  {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking}, //TODO: comma not or?
                  {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
               }
               , target.Table.Detail.Schema
               , target.Table.Detail.Name
               , target.Target.Options.ScriptAsDelimitedImpliedInterfaces() // TODO: unsure about this
            )
            .Ignore();
      }

      private static void CloseClass(TableTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"    }"}
               })
            .Ignore();
      }
   }
}
