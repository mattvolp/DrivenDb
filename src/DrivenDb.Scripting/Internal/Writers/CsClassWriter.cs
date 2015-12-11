using System.Collections.Generic;
using DrivenDb.Data;
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

      public void Write(ScriptTarget target, IReadOnlyCollection<TableMap> tables)
      {
         foreach (var table in tables)
         {
            Write(target, table);
         }
      }

      public void Write(ScriptTarget target, TableMap table)
      {
         target.WriteLines(new ScriptLines()
            {
               {"                                                                   "},
               {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
               {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
               {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
               {"    public partial class $1 $2                                     "},
               {"    {                                                              "},
               {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking},
               {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
            }
            , table.Detail.Schema
            , table.Detail.Name
            , target.Options.ScriptAsDelimitedImpliedInterfaces() // TODO: unsure about this
            );

         _content.Write(target, table);
         
         target.WriteLine("    }");
      }      
   }
}
