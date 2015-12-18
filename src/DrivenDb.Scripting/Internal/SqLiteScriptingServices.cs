using System;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class SqLiteScriptingServices
   {
      // TODO: the similarities between this and MsSqlType leads me to believe this can be further reduced
      public static string ToScriptedDefaultValue(ScriptingOptions options, ColumnDetail column)
      {
         var value = DbScriptingServices.StripParentheses(column.DefaultValue);

         if (DbScriptingServices.DefaultIsExplicitNull(value))
         {
            value = DbScriptingServices.ScriptCsNull();
         }
         else
         {
            if (DbScriptingServices.DefaultIsSingleQuoted(value))
            {
               value = DbScriptingServices.ReplaceSingleQuotes(value);
            }

            if (DbScriptingServices.DefaultIsGetDateFunction(value))
            {
               value = DbScriptingServices.ScriptCsNowFunction(options, column.SqlType.IsDateOnly());
            }
            else if (ColumnIsDateOrTimeType(column))
            {
               value = DbScriptingServices.ScriptCsDateTimeParse(options, column.SqlType.IsDateOnly(), value);
            }
            else if (ColumnIsBoolType(column))
            {
               value = DbScriptingServices.ScriptCsBoolFromNumeric(value);
            }
            else if (ColumnIsDecimalType(column))
            {
               value = DbScriptingServices.ScriptCsDecimalFromNumeric(value);
            }
         }

         return value;
      }

      private static bool ColumnIsDateOrTimeType(ColumnDetail column)
      {
         return column.SqlType.ToString().StartsWith("date", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool DefaultIsGetDateFunction(string value)
      {
         return value.StartsWith("current_timestamp", StringComparison.CurrentCultureIgnoreCase)
             || value.StartsWith("datetime('now')", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool ColumnIsBoolType(ColumnDetail column)
      {
         return String.Compare(column.SqlType.ToString(), "bool", StringComparison.CurrentCultureIgnoreCase) == 0;
      }

      private static bool ColumnIsDecimalType(ColumnDetail column)
      {
         return String.Compare(column.SqlType.ToString(), "decimal", StringComparison.CurrentCultureIgnoreCase) == 0
             || String.Compare(column.SqlType.ToString(), "numeric", StringComparison.CurrentCultureIgnoreCase) == 0;
      }      
   }
}
