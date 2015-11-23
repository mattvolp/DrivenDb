using System;
using System.Linq;
using DrivenDb.Data.Internal;

namespace DrivenDb.Data
{
   internal abstract class DbType
   {
      protected readonly string _sqlType;
      protected readonly string _csType;
      protected readonly bool _isDateOnly;

      protected DbType(string sqlType, string csType, bool isDateOnly)
         : this(sqlType, csType)
      {         
         _isDateOnly = isDateOnly;
      }

      protected DbType(string sqlType, string csType)
      {
         _sqlType = sqlType;
         _csType = csType;
         _isDateOnly = false;
      }

      // TODO: needed?
      public override bool Equals(object other)
      {
         var dbType = other as DbType;

         if (dbType == null)
         {
            return false;
         }

         return Equals(_sqlType, dbType._sqlType);         
      }

      // TODO: needed?
      public override int GetHashCode()
      {
         return _sqlType.GetHashCode();
      }

      // TODO: needed?
      public override string ToString()
      {
         return _sqlType;
      }

      public string ToCsString()
      {
         return _csType;
      }

      public bool IsDateOnly()
      {
         return _isDateOnly;
      }

      public abstract string ToCsDefault(ScriptingOptions options, ColumnDetail column);

      protected static string StripParentheses(string value)
      {
         while (value.Length >= 2 && value.First() == '(' && value.Last() == ')')
         {
            value = value.Substring(1, value.Length - 2);
         }

         return value.Trim();
      }

      protected static string ReplaceSingleQuotes(string value)
      {
         return "\"" + value.Substring(1, value.Length - 2) + "\"";
      }

      protected static bool DefaultIsExplicitNull(string value)
      {
         return String.Compare(value, "null", StringComparison.CurrentCultureIgnoreCase) == 0;
      }

      protected static bool DefaultIsSingleQuoted(string value)
      {
         return value[0] == '\'';
      }

      protected static string ScriptCsBoolFromNumeric(string value)
      {
         value = value == "1"
            ? "true"
            : "false";

         return value;
      }

      protected static string ScriptCsNull()
      {
         return "null";
      }

      protected static string ScriptCsDecimalFromNumeric(string value)
      {         
         return value += "m";
      }

      protected static string ScriptCsNowFunction(ScriptingOptions options, bool isDateOnlyType)
      {
         var value = options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
               ? "new DateTime(DateTime.Now{0}.Ticks, DateTimeKind.Unspecified)"
               : "DateTime.Now{0}";

         value = isDateOnlyType && options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns)
            ? String.Format(value, ".Date")
            : String.Format(value, "");

         return value;
      }

      protected static string ScriptCsDateTimeParse(ScriptingOptions options, bool isDateOnly, string value)
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
