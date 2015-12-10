using System;
using DrivenDb.Data;

namespace DrivenDb.Data.SqLite
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
