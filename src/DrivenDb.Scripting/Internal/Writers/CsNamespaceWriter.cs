using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;
using DrivenDb.Core.Extensions;

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

      public TablesTarget Write(TablesTarget target)
      {
         return target
            .Chain(OpenNamespace)
            .Hitch(_content.Write)
            .Chain(CloseNamespace);         
      }

      private static void OpenNamespace(TablesTarget target)
      {
         target.Writer.WriteLines(new ScriptLines()
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
            }, target.Target.Namespace)
            .Ignore();
      }

      private static void CloseNamespace(TablesTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"}"}
               })
            .Ignore();
      }  
   }
}
