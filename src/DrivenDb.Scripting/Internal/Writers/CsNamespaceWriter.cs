using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsNamespaceWriter
      : ITablesWriter
   {
      private readonly ITablesWriter _content;
      
      public CsNamespaceWriter(ITablesWriter content)
      {
         _content = content;         
      }

      public void Write(ScriptTarget target, IReadOnlyCollection<TableMap> tables)
      {
         target.WriteLines(new ScriptLines()
            {
               {"namespace $0                                "},
               {"{                                           "},
               {"    using DrivenDb.Core;                    "},
               {"    using System;                           "},
               {"    using System.Linq;                      "},
               {"    using System.Collections.Generic;       "},
               {"    using System.Runtime.CompilerServices;  "},
               {"    using System.Data.Linq;                 ", ScriptingOptions.ImplementLinqContext},
               {"    using System.Data.Linq.Mapping;         ", ScriptingOptions.ImplementLinqContext},
               {"    using System.Runtime.Serialization;     ", ScriptingOptions.Serializable},
               {"    using System.ComponentModel;            ", ScriptingOptions.ImplementNotifyPropertyChanged, ScriptingOptions.ImplementNotifyPropertyChanging},
            }, target.Namespace);

         _content.Write(target, tables);
         
         target.WriteLine("}");
      }      
   }
}
