using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsNamespaceScripter      
   {
      private readonly IScripter<NamespaceDetail> _content;

      public CsNamespaceScripter(IScripter<NamespaceDetail> content)
      {
         _content = content;
      }

      public Script<NamespaceDetail> Script(NamespaceDetail d, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<NamespaceDetail>(d, so, sc)
            .Bind(WriteNamespaceOpen)
            .Bind(_content)            
            .Bind(WriteNamespaceClose);
      }
      
      private static Script<NamespaceDetail> WriteNamespaceOpen(NamespaceDetail nd, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<NamespaceDetail>(nd, so,
            sc.Append(new ScriptSegment(nd.Namespace)
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
               }));
      }
      
      private static Script<NamespaceDetail> WriteNamespaceClose(NamespaceDetail nd, ScriptingOptions so, SegmentCollection sc)
      {
         return new Script<NamespaceDetail>(nd, so,
            sc.Append(new ScriptSegment()
               {
                  {"}"}
               }));
      } 
   }
}
