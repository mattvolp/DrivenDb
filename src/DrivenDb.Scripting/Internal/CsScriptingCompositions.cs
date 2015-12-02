using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class CsScriptingCompositions
   {
      public static ScriptTarget WriteKeyClassAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            CsScriptingServices.WriteKeyClass(target, table);

         return target;
      }

      public static ScriptTarget WriteClassOpenAndContinue(this ScriptTarget target, TableMap table)
      {         
         CsScriptingServices.WriteClassOpen(target, table.Detail);
         return target;
      }

      public static ScriptTarget WriteConstructorAndContinue(this ScriptTarget target, TableMap table)
      {
         CsScriptingServices.WriteConstructor(target, table);
         return target;
      }

      public static ScriptTarget WriteFieldsAndContinue(this ScriptTarget target, TableMap table)
      {
         CsScriptingServices.WriteFields(target, table.Columns);
         return target;
      }

      public static ScriptTarget WriteKeyPropertyAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            CsScriptingServices.WriteKeyProperty(target, table);

         return target;
      }

      public static ScriptTarget WritePropertiesAndContinue(this ScriptTarget target, TableMap table)
      {
         CsScriptingServices.WriteProperties(target, table.Columns);
         return target;
      }

      public static ScriptTarget WritePartialsAndContinue(this ScriptTarget target, TableMap table)
      {
         CsScriptingServices.WritePartials(target, table.Columns);
         return target;
      }

      public static ScriptTarget WritePropertyChangingAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
            CsScriptingServices.WritePropertyChanging(target);

         return target;
      }

      public static ScriptTarget WritePropertyChangedAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
            CsScriptingServices.WritePropertyChanged(target);

         return target;
      }

      public static ScriptTarget WriteValidationCheckAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementValidationCheck))
            CsScriptingServices.WriteValidationCheck(target, table);

         return target;
      }

      public static ScriptTarget WriteClassCloseAndContinue(this ScriptTarget target, TableMap table)
      {
         CsScriptingServices.WriteClassClose(target);
         return target;
      }
   }
}
