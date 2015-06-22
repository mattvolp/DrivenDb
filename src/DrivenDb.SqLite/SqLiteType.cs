using System;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.SqLite
{
    internal class SqLiteType
       : DbType
    {
       public SqLiteType(string sqlType, bool isNullable)
          : base(sqlType, ToCsType(sqlType, isNullable), IsDateOnly(sqlType))
       {
       }

       public static SqLiteType Integer = new SqLiteType("integer", false);
       public static SqLiteType NullableInteger = new SqLiteType("integer", true);
       public static SqLiteType Text = new SqLiteType("text", false);
       public static SqLiteType NullableText = new SqLiteType("text", true);
       public static SqLiteType Real = new SqLiteType("real", false);
       public static SqLiteType NullableReal = new SqLiteType("real", true);
       public static SqLiteType Numeric = new SqLiteType("numeric", false);
       public static SqLiteType NullableNumeric = new SqLiteType("numeric", true);

       // TODO: the similarities between this and MsSqlType leads me to believe this can be further reduced
       public override string ToCsDefault(ScriptingOptions options, ColumnDetail column)
       {
          var value = StripParentheses(column.DefaultValue);

          if (DefaultIsExplicitNull(value))
          {
             value = ScriptCsNull();
          }
          else
          {
             if (DefaultIsSingleQuoted(value))
             {
                value = ReplaceSingleQuotes(value);
             }

             if (DefaultIsGetDateFunction(value))
             {
                value = ScriptCsNowFunction(options, IsDateOnly());
             }
             else if (ColumnIsDateOrTimeType(column))
             {
                value = ScriptCsDateTimeParse(options, IsDateOnly(), value);
             }
             else if (ColumnIsBoolType(column))
             {
                value = ScriptCsBoolFromNumeric(value);
             }
             else if (ColumnIsDecimalType(column))
             {
                value = ScriptCsDecimalFromNumeric(value);
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

       private static bool IsDateOnly(string sqlType)
       {
          return String.Compare(sqlType, "date", StringComparison.CurrentCultureIgnoreCase) == 0;
       }

       private static string ToCsType(string sqlType, bool isNullable)
       {          
          var csType = "";

          if (
                  sqlType.IndexOf("int", StringComparison.CurrentCultureIgnoreCase) >= 0
               )
          {
             if (
                   sqlType.IndexOf("small", StringComparison.CurrentCultureIgnoreCase) >= 0
                || sqlType.IndexOf("tiny", StringComparison.CurrentCultureIgnoreCase) >= 0
                )
             {
                csType = "short";
             }
             else if
                (
                   sqlType.IndexOf("big", StringComparison.CurrentCultureIgnoreCase) >= 0
                || sqlType.IndexOf("8", StringComparison.CurrentCultureIgnoreCase) >= 0
                )
             {
                csType = "long";
             }
             else
             {
                csType = "int";
             }
          }
          else if
             (
                sqlType.IndexOf("char", StringComparison.CurrentCultureIgnoreCase) >= 0
             || sqlType.IndexOf("text", StringComparison.CurrentCultureIgnoreCase) >= 0
             || sqlType.IndexOf("clob", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "string";
          }
          else if
             (
                sqlType.IndexOf("blob", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "byte[]";
          }
          else if
             (
                sqlType.IndexOf("real", StringComparison.CurrentCultureIgnoreCase) >= 0
             || sqlType.IndexOf("double", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "double";
          }
          else if
             (
                sqlType.IndexOf("float", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "float";
          }
          else if
             (
                sqlType.IndexOf("date", StringComparison.CurrentCultureIgnoreCase) >= 0
             || sqlType.IndexOf("time", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "DateTime";
          }
          else if
             (
                sqlType.IndexOf("bool", StringComparison.CurrentCultureIgnoreCase) >= 0
             )
          {
             csType = "bool";
          }
          else
          {
             csType = "decimal";
          }

          if (csType != "byte[]" && csType != "string" && isNullable)
          {
             csType += "?";
          }

          return csType;
       }
    }
}
