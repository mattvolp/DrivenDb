using System;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Data.MsSql;

namespace DrivenDb.Scripting.Internal
{
   internal static class CsValueScriptingServices
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
         var value = StripParentheses(column.DefaultValue);

         return DefaultIsExplicitNull(value)
            ? ScriptCsNull()
            : value.ScriptExplicitValue(options, column);
      }

      public static string ScriptExplicitValue(this string value, ScriptingOptions options, ColumnDetail column)
      {
         return value.DefaultIsString()
            ? value.ReplaceQuotes()
            : value.DefaultIsDate(column)
               ? value.ScriptDateValue(options, column)
               : ColumnIsBooleanType(column)
                  ? ScriptCsBoolFromNumeric(value)
                  : ColumnIsDecimalType(column)
                     ? ScriptCsDecimalFromNumeric(value)
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
         return DefaultIsGetDateFunction(value) || ColumnIsDateOrTimeType(column);
      }

      public static string ScriptDateValue(this string value, ScriptingOptions options, ColumnDetail column)
      {
         return DefaultIsGetDateFunction(value)
            ? ScriptCsNowFunction(options, column.SqlType.Equals(MsSqlType.Date))
            : ColumnIsDateOrTimeType(column)
               ? value = ScriptCsDateTimeParse(options, column.SqlType.Equals(MsSqlType.Date), value)
               : value;
      }

      public static string ReplaceUnicodeQuotes(this string value)
      {
         return "\"" + value.Substring(2, value.Length - 3) + "\"";
      }

      private static string ReplaceAnsiQuotes(this string value)
      {
         return ReplaceSingleQuotes(value);
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

      private static bool DefaultIsGetDateFunction(string value)
      {
         return value.StartsWith("getdate()", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool DefaultIsUnicodeString(string value)
      {
         return value.StartsWith("N\'", StringComparison.CurrentCultureIgnoreCase);
      }

      private static bool DefaultIsAnsiString(string value)
      {
         return DefaultIsSingleQuoted(value);
      }

      public static string StripParentheses(string value)
      {
         while (value.Length >= 2 && value.First() == '(' && value.Last() == ')')
         {
            value = value.Substring(1, value.Length - 2);
         }

         return value.Trim();
      }

      public static string ReplaceSingleQuotes(string value)
      {
         return "\"" + value.Substring(1, value.Length - 2) + "\"";
      }

      public static bool DefaultIsExplicitNull(string value)
      {
         return String.Compare(value, "null", StringComparison.CurrentCultureIgnoreCase) == 0;
      }

      public static bool DefaultIsSingleQuoted(string value)
      {
         return value[0] == '\'';
      }

      public static string ScriptCsBoolFromNumeric(string value)
      {
         value = value == "1"
            ? "true"
            : "false";

         return value;
      }

      public static string ScriptCsNull()
      {
         return "null";
      }

      public static string ScriptCsDecimalFromNumeric(string value)
      {
         return value += "m";
      }

      public static string ScriptCsNowFunction(ScriptingOptions options, bool isDateOnlyType)
      {
         var value = options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
               ? "new DateTime(DateTime.Now{0}.Ticks, DateTimeKind.Unspecified)"
               : "DateTime.Now{0}";

         value = isDateOnlyType && options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns)
            ? String.Format(value, ".Date")
            : String.Format(value, "");

         return value;
      }

      public static string ScriptCsDateTimeParse(ScriptingOptions options, bool isDateOnly, string value)
      {
         var parsed = isDateOnly && options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns)
            ? $"DateTime.Parse({value}).Date"
            : $"DateTime.Parse({value})";

         value = options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
            ? $"new DateTime({parsed}.Ticks, DateTimeKind.Unspecified)"
            : parsed;

         return value;
      }
   }
}
