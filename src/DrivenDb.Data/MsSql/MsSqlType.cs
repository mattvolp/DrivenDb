using System;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

// ReSharper disable once CheckNamespace
namespace DrivenDb.MsSql
{
   internal class MsSqlType
      : DbType
   {
      internal MsSqlType(string sqlType, string csType, bool isDateOnly)
         : base(sqlType, csType, isDateOnly)
      {
      }

      internal MsSqlType(string sqlType, string csType)
         : base(sqlType, csType)
      {
      }

      // TODO: append "?" to sql types needs to be removed.  these are static definitions and nothing depends on it anymore. force true parameters. delete other constructor.
      //       base type overrides of gethashcode and equals would then be awkward and maybe should be removed
      public static MsSqlType BigInt = new MsSqlType("bigint", "long");
      public static MsSqlType NullableBigInt = new MsSqlType("bigint?", "long?");
      public static MsSqlType Bit = new MsSqlType("bit", "bool");
      public static MsSqlType NullableBit = new MsSqlType("bit?", "bool?");
      public static MsSqlType Char = new MsSqlType("char", "string");
      public static MsSqlType NullableChar = new MsSqlType("char?", "string");
      public static MsSqlType Date = new MsSqlType("date", "DateTime", true);
      public static MsSqlType NullableDate = new MsSqlType("date?", "DateTime?", true);
      public static MsSqlType DateTime = new MsSqlType("datetime", "DateTime");
      public static MsSqlType NullableDatetime = new MsSqlType("datetime?", "DateTime?");
      public static MsSqlType DateTime2 = new MsSqlType("datetime2", "DateTime");
      public static MsSqlType NullableDatetime2 = new MsSqlType("datetime2?", "DateTime?");
      public static MsSqlType Decimal = new MsSqlType("decimal", "decimal");
      public static MsSqlType NullableDecimal = new MsSqlType("decimal?", "decimal?");
      public static MsSqlType Float = new MsSqlType("float", "double");
      public static MsSqlType NullableFloat = new MsSqlType("float?", "double?");
      public static MsSqlType Image = new MsSqlType("image", "byte[]");
      public static MsSqlType NullableImage = new MsSqlType("image?", "byte[]");
      public static MsSqlType Int = new MsSqlType("int", "int");
      public static MsSqlType NullableInt = new MsSqlType("int?", "int?");
      public static MsSqlType Money = new MsSqlType("money", "decimal");
      public static MsSqlType NullableMoney = new MsSqlType("money?", "decimal?");
      public static MsSqlType Nchar = new MsSqlType("nchar", "string");
      public static MsSqlType NullableNchar = new MsSqlType("nchar?", "string");
      public static MsSqlType Numeric = new MsSqlType("numeric", "decimal");
      public static MsSqlType NullableNumeric = new MsSqlType("numeric?", "decimal?");
      public static MsSqlType Nvarchar = new MsSqlType("nvarchar", "string");
      public static MsSqlType NullableNvarchar = new MsSqlType("nvarchar?", "string");
      public static MsSqlType Real = new MsSqlType("real", "single");
      public static MsSqlType NullableReal = new MsSqlType("real?", "single?");
      public static MsSqlType SmallDateTime = new MsSqlType("smalldatetime", "DateTime");
      public static MsSqlType NullableSmallDateTime = new MsSqlType("smalldatetime?", "DateTime?");
      public static MsSqlType SmallInt = new MsSqlType("smallint", "short");
      public static MsSqlType NullableSmallInt = new MsSqlType("smallint?", "short?");
      public static MsSqlType Text = new MsSqlType("text", "string");
      public static MsSqlType NullableText = new MsSqlType("text?", "string");
      public static MsSqlType Timestamp = new MsSqlType("timestamp", "byte[]");
      public static MsSqlType NullableTimestamp = new MsSqlType("timestamp?", "byte[]");
      public static MsSqlType Time = new MsSqlType("time", "TimeSpan");
      public static MsSqlType NullableTime = new MsSqlType("time?", "TimeSpan?");
      public static MsSqlType TinyInt = new MsSqlType("tinyint", "byte");
      public static MsSqlType NullableTinyInt = new MsSqlType("tinyint?", "byte?");
      public static MsSqlType UniqueIdentifier = new MsSqlType("uniqueidentifier", "Guid");
      public static MsSqlType NullableUniqueIdentifier = new MsSqlType("uniqueidentifier?", "Guid?");
      public static MsSqlType Varbinary = new MsSqlType("varbinary", "byte[]");
      public static MsSqlType NullableVarbinary = new MsSqlType("varbinary?", "byte[]");
      public static MsSqlType Varchar = new MsSqlType("varchar", "string");
      public static MsSqlType NullableVarchar = new MsSqlType("varchar?", "string");
      public static MsSqlType Xml = new MsSqlType("xml", "string");
      public static MsSqlType NullableXml = new MsSqlType("xml?", "string");   

      public override string ToCsDefault(ScriptingOptions options, ColumnDetail column)
      {
         /*
        * (N'TEST')
        * ('TEST')
        * ((5))
        * (getdate())
        */
         var value = StripParentheses(column.DefaultValue);

         if (DefaultIsExplicitNull(value))
         {
            value = ScriptCsNull();
         }
         else
         {
            if (DefaultIsAnsiString(value))
            {
               value = ReplaceAnsiQuotes(value);
            }
            else if (DefaultIsUnicodeString(value))
            {
               value = ReplaceUnicodeQuotes(value);
            }

            if (DefaultIsGetDateFunction(value))
            {
               // TODO: doesn't cover timespan defaults?
               value = ScriptCsNowFunction(options, IsDateOnly());
            }                        
            else if (ColumnIsDateOrTimeType(column))
            {
               // TODO: doesn't cover timespan defaults
               value = ScriptCsDateTimeParse(options, IsDateOnly(), value);
            }
            else if (ColumnIsBooleanType(column))
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

      private static string ReplaceUnicodeQuotes(string value)
      {
         return "\"" + value.Substring(2, value.Length - 3) + "\"";
      }

      private static string ReplaceAnsiQuotes(string value)
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
   }   
}

