using System.Collections.Generic;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class CsUnitScriptingCompositions
   {
      public static ScriptTarget WriteNamespaceOpenAndContinue(this ScriptTarget target, string @namespace)
      {
         CsUnitScriptingServices.WriteNamespaceOpen(target, @namespace);
         return target;
      }

      public static ScriptTarget WriteContextAndContinue(this ScriptTarget target, string contextName, IEnumerable<TableDetail> tables)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementLinqContext))
            CsUnitScriptingServices.WriteContext(target, contextName, tables);
         return target;
      }

      public static ScriptTarget WriteEntitiesAndContinue(this ScriptTarget target, IEnumerable<TableMap> tables)
      {
         CsUnitScriptingServices.WriteEntities(target, tables);
         return target;
      }

      public static void WriteNamespaceClose(this ScriptTarget target)
      {
         CsUnitScriptingServices.WriteNamespaceClose(target);         
      }

      //
      //
      //

      public static ScriptTarget WriteKeyClassAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            CsUnitScriptingServices.WriteKeyClass(target, table);

         return target;
      }
      
      public static ScriptTarget WriteClassOpenAndContinue(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WriteClassOpen(target, table.Detail);
         return target;
      }
      
      public static ScriptTarget WriteConstructorAndContinue(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WriteConstructor(target, table);
         return target;
      }
      
      public static ScriptTarget WriteFieldsAndContinue(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WriteFields(target, table.Columns);
         return target;
      }
      
      public static ScriptTarget WriteKeyPropertyAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            CsUnitScriptingServices.WriteKeyProperty(target, table);

         return target;
      }
      
      public static ScriptTarget WritePropertiesAndContinue(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WriteProperties(target, table.Columns);
         return target;
      }
      
      public static ScriptTarget WritePartialsAndContinue(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WritePartials(target, table.Columns);
         return target;
      }
      
      public static ScriptTarget WritePropertyChangingAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
            CsUnitScriptingServices.WritePropertyChanging(target);

         return target;
      }
      
      public static ScriptTarget WritePropertyChangedAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
            CsUnitScriptingServices.WritePropertyChanged(target);

         return target;
      }
      
      public static ScriptTarget WriteValidationCheckAndContinue(this ScriptTarget target, TableMap table)
      {
         if (target.Options.HasFlag(ScriptingOptions.ImplementValidationCheck))
            CsUnitScriptingServices.WriteValidationCheck(target, table);

         return target;
      }
      
      public static void WriteClassClose(this ScriptTarget target, TableMap table)
      {
         CsUnitScriptingServices.WriteClassClose(target);
      }

      //public static Tuple<ScriptTarget, TableMap> WriteKeyClassAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   if (target.Item1.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
      //      CsUnitScriptingServices.WriteKeyClass(target.Item1, target.Item2);

      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WriteClassOpenAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WriteClassOpen(target.Item1, target.Item2.Detail);
      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WriteConstructorAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WriteConstructor(target.Item1, target.Item2);
      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WriteFieldsAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WriteFields(target.Item1, target.Item2.Columns);
      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WriteKeyPropertyAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   if (target.Item1.Options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
      //      CsUnitScriptingServices.WriteKeyProperty(target.Item1, target.Item2);

      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WritePropertiesAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WriteProperties(target.Item1, target.Item2.Columns);
      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WritePartialsAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WritePartials(target.Item1, target.Item2.Columns);
      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WritePropertyChangingAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   if (target.Item1.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
      //      CsUnitScriptingServices.WritePropertyChanging(target.Item1);

      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WritePropertyChangedAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   if (target.Item1.Options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
      //      CsUnitScriptingServices.WritePropertyChanged(target.Item1);

      //   return target;
      //}

      //public static Tuple<ScriptTarget, TableMap> WriteValidationCheckAndContinue(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   if (target.Item1.Options.HasFlag(ScriptingOptions.ImplementValidationCheck))
      //      CsUnitScriptingServices.WriteValidationCheck(target.Item1, target.Item2);

      //   return target;
      //}

      //public static void WriteClassClose(this Tuple<ScriptTarget, TableMap> target)
      //{
      //   CsUnitScriptingServices.WriteClassClose(target.Item1);
      //}
   }
}
