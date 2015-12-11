using System;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Data.MsSql;

namespace DrivenDb.Scripting.Internal
{
   internal class CsDefaultTranslator
   {
      private readonly ScriptingOptions _options;

      public CsDefaultTranslator(ScriptingOptions options)
      {
         _options = options;
      }

      public string Translate(ColumnMap column)
      {
         return MsSqlScriptingServices.ToCsScriptedDefaultValue(_options, column.Detail);
      }
   }

   internal static class MsSqlScriptingServices
   {
      // TODO: this is really MS SQL TO CS sspecific stuff
      public static string ToCsScriptedDefaultValue(ScriptingOptions options, ColumnDetail column)
      {
         /*
          * (N'TEST')
          * ('TEST')
          * ((5))
          * (getdate())
          */
         var value = DbScriptingServices.StripParentheses(column.DefaultValue);

         return DbScriptingServices.DefaultIsExplicitNull(value)
            ? DbScriptingServices.ScriptCsNull()
            : value.ScriptExplicitValue(options, column);
      }

      public static string ScriptExplicitValue(this string value, ScriptingOptions options, ColumnDetail column)
      {
         return value.DefaultIsString()
            ? value.ReplaceQuotes()
            : value.DefaultIsDate(column)
               ? value.ScriptDateValue(options, column)
               : ColumnIsBooleanType(column)
                  ? DbScriptingServices.ScriptCsBoolFromNumeric(value)
                  : ColumnIsDecimalType(column)
                     ? DbScriptingServices.ScriptCsDecimalFromNumeric(value)
                     : value;

            //if (DefaultIsAnsiString(value))
            //{
            //   value = ReplaceAnsiQuotes(value);
            //}
            //else if (DefaultIsUnicodeString(value))
            //{
            //   value = ReplaceUnicodeQuotes(value);
            //}

         //if (DefaultIsGetDateFunction(value))
         //{
         //   // TODO: doesn't cover timespan defaults?
         //   value = ScriptCsNowFunction(options, column.SqlType.Equals(MsSqlType.Date));
         //}
         //else if (ColumnIsDateOrTimeType(column))
         //{
         //   // TODO: doesn't cover timespan defaults
         //   value = ScriptCsDateTimeParse(options, column.SqlType.Equals(MsSqlType.Date), value);
         //}
         //else if (ColumnIsBooleanType(column))
         //{
         //   value = ScriptCsBoolFromNumeric(value);
         //}
         //else if (ColumnIsDecimalType(column))
         //{
         //   value = ScriptCsDecimalFromNumeric(value);
         //}
      }

      public static bool DefaultIsString(this string value)
      {
         return DefaultIsAnsiString(value) || DefaultIsUnicodeString(value);
      }

      public static string ReplaceQuotes(this string value)
      {
         return DefaultIsAnsiString(value)
            ? ReplaceAnsiQuotes(value)
            : DefaultIsUnicodeString(value)
               ? ReplaceUnicodeQuotes(value)
               : value;
      }

      public static bool DefaultIsDate(this string value, ColumnDetail column)
      {
         return DbScriptingServices.DefaultIsGetDateFunction(value) || ColumnIsDateOrTimeType(column);
      }

      public static string ScriptDateValue(this string value, ScriptingOptions options, ColumnDetail column)
      {
         return DbScriptingServices.DefaultIsGetDateFunction(value)
            ? DbScriptingServices.ScriptCsNowFunction(options, column.SqlType.Equals(MsSqlType.Date))
            : ColumnIsDateOrTimeType(column)
               ? DbScriptingServices.ScriptCsDateTimeParse(options, column.SqlType.Equals(MsSqlType.Date), value)
               : value;
      }

      public static string ReplaceUnicodeQuotes(this string value)
      {
         return "\"" + value.Substring(2, value.Length - 3) + "\"";
      }

      private static string ReplaceAnsiQuotes(this string value)
      {
         return DbScriptingServices.ReplaceSingleQuotes(value);
      }

      private static bool ColumnIsDateOrTimeType(ColumnDetail column)
      {
         return column.SqlType.ToString().StartsWith("date")
             || column.SqlType.ToString() == "time";
      }

      private static bool ColumnIsDecimalType(ColumnDetail column)
      {
         return column.SqlType.ToString()
            .StartsWith("decimal", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool ColumnIsBooleanType(ColumnDetail column)
      {
         return column.SqlType.ToString()
            .StartsWith("bit", StringComparison.CurrentCultureIgnoreCase);
      }
      
      private static bool DefaultIsUnicodeString(string value)
      {
         return value.StartsWith("N\'", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool DefaultIsAnsiString(string value)
      {
         return DbScriptingServices.DefaultIsSingleQuoted(value);
      }

      

      

      

      

      

      

      
   }
}
