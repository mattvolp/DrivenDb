using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class DbScriptingServices
   {
      public static string StripParentheses(string value)
      {
         while (value.Length >= 2 && value.First() == '(' && value.Last() == ')')
         {
            value = value.Substring(1, value.Length - 2);
         }

         return value.Trim();
      }

      public static bool DefaultIsExplicitNull(string value)
      {
         return String.Compare(value, "null", StringComparison.CurrentCultureIgnoreCase) == 0;
      }

      public static string ScriptCsNull()
      {
         return "null";
      }

      public static bool DefaultIsSingleQuoted(string value)
      {
         return value[0] == '\'';
      }

      public static string ReplaceSingleQuotes(string value)
      {
         return "\"" + value.Substring(1, value.Length - 2) + "\"";
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

      public static string ScriptCsBoolFromNumeric(string value)
      {
         value = value == "1"
            ? "true"
            : "false";

         return value;
      }
      
      public static string ScriptCsDecimalFromNumeric(string value)
      {
         return value + "m";
      }

      public static bool DefaultIsGetDateFunction(string value)
      {
         return value.StartsWith("getdate()", StringComparison.CurrentCultureIgnoreCase);
      }
   }
}
