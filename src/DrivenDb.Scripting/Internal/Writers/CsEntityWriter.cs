using System.Collections.Generic;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Writers
{
   // TODO: this class feel extraneous
   internal class CsEntityWriter
   {
      private readonly CsClassWriter _classes;

      public CsEntityWriter(CsClassWriter classes)
      {
         _classes = classes;
      }

      public void Write(ScriptTarget target, IEnumerable<TableMap> tables)
      {         
         foreach (var table in tables)
         {
            Write(target, table);
         }
      }

      public void Write(ScriptTarget target, TableMap table)
      {
         _classes.Write(target, table);
         //target.WriteKeyClassAndContinue(table)
         //   .WriteClassOpenAndContinue(table)
         //   .WriteConstructorAndContinue(table)
         //   .WriteFieldsAndContinue(table)
         //   .WriteKeyPropertyAndContinue(table)
         //   .WritePropertiesAndContinue(table)
         //   .WritePartialsAndContinue(table)
         //   .WritePropertyChangingAndContinue(table)
         //   .WritePropertyChangedAndContinue(table)
         //   .WriteValidationCheckAndContinue(table)
         //   .WriteClassClose(table);

      }
   }
}
