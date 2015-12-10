﻿using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPartialWriter
   {
      public void Write(ScriptTarget target, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            Write(target, column);
         }
      }

      public void Write(ScriptTarget target, ColumnMap column)
      {
         target.WriteLines(new ScriptLines()
            {
               {"                                                                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges},
            }
            , column.Detail.Name
            , column.ScriptAsCsType());
      }
   }
}
