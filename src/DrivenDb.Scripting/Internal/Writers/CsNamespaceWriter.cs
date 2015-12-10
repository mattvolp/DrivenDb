using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsNamespaceWriter
   {
      private readonly CsContextWriter _context;
      private readonly CsEntityWriter _entities;

      public CsNamespaceWriter(
         // CsContentWriterCollection IEnumerable<Target, Tables>?
           CsContextWriter context
         , CsEntityWriter entities
         )
      {
         _context = context;
         _entities = entities;
      }

      public void Write(ScriptTarget target, string @namespace, string contextName, TableMap[] tables)
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
            }, @namespace);

         _context.Write(target, contextName, tables.GetDetails());
         _entities.Write(target, tables);

         target.WriteLine("}");
      }      
   }
}
