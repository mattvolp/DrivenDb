using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class Extensions
   {      
      private static readonly Regex ArgsEx = new Regex(@"\$(\d)");

      //
      // SPECIALIZED STRINGS
      //

      public static string FromDollarToStringFormat(this string input)
      {
         return input
            .Replace("{", "\\{")
            .Replace("}", "\\}")
            .FromDollarToStringFormatArguments()
            .Replace("\\{", "{{")
            .Replace("\\}", "}}");
      }

      public static string FromDollarToStringFormatArguments(this string input)
      {
         // changes "this code $0" to "this code {0}"
         return ArgsEx.Replace(input, "{$1}");
      }
      
      // 
      // STRINGS
      //

      public static string Format(this string format, params object[] args)
      {
         return string.Format(format, args);
      }

      public static string Join<T>(this IEnumerable<T> items, string delimiter)
      {
         return string.Join(delimiter, items);
      }

      //
      // SCRIPTING
      //

      public static void GuardAgainstKeyClassOverflow(this IEnumerable<ColumnMap> columns)
      {
         if (columns.Count() > 8)
            throw new Exception("Unable to script key class for tables with a primary key of more than 8 columns");
      }

      public static string ScriptAsCsBoolean(this bool b)
      {
         return b ? "true" : "false";
      }

      public static string ScriptAsDelimitedImpliedInterfaces(this ScriptingOptions options)
      {
         var interfaces = options.ExtractImpliedInterfaces()
            .ToArray();

         return interfaces.Any()
            ? ": " + interfaces.Join(", ")
            : "";
      }

      public static IEnumerable<string> ExtractImpliedInterfaces(this ScriptingOptions options)
      {         
         if (options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
         {
            yield return "INotifyPropertyChanging";
         }

         if (options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
         {
            yield return "INotifyPropertyChanged";
         }

         if (options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            yield return "IDbEntityProvider";
         }
      }

      public static string ScriptAsDelimitedCsTypes(this IEnumerable<ColumnMap> columns)
      {
         return columns
            .Select(ScriptAsCsType)
            .Join(", ");
      }

      public static string ScriptAsCsType(this ColumnMap column)
      {
         return column.HasCustomType
            ? column.CustomType
            : column.Detail.SqlType.ToCsString();
      }

      public static string ScriptAsDelimitedCsTypedParameterNames(this IEnumerable<ColumnMap> columns)
      {
         return columns
            .Select(ScriptAsCsTypedParameterName)
            .Join(", ");
      }

      public static string ScriptAsCsTypedParameterName(this ColumnMap column)
      {
         return column.Detail.SqlType.ToCsString() + " @" + column.Detail.Name.ToLower();
      }

      public static string ScriptAsDelimitedParameterNames(this IEnumerable<ColumnMap> columns)
      {
         return columns
            .Select(ScriptAsParameterName)
            .Join(", ");
      }

      public static string ScriptAsParameterName(this ColumnMap column)
      {
         return "@" + column.Detail.Name.ToLower();
      }

      public static string ScriptAsDelimitedPrivateMemberNames(this IEnumerable<ColumnMap> columns)
      {
         return columns
            .Select(ScriptAsPrivateMemberName)
            .Join(", ");
      }

      public static string ScriptAsPrivateMemberName(this ColumnMap column)
      {
         return "_" + column.Detail.Name;
      }

      //
      // TABLE RELATED
      //

      public static IEnumerable<ColumnMap> GetColumnsWithDefaultDefinitions(this TableMap table)
      {
         foreach (var column in table.Columns)
         {
            if (column.Detail.HasDefault)
               yield return column;
         }
      }

      public static IEnumerable<ColumnMap> GetPrimaryKeyColumns(this TableMap table)
      {
         foreach (var column in table.Columns)
         {
            if (column.Detail.IsPrimary)
               yield return column;
         }         
      }

      public static IEnumerable<ColumnMap> GetRequiredColumns(this TableMap table)
      {
         foreach (var column in table.Columns)
         {
            if (   !column.Detail.IsGenerated
                && !column.Detail.IsNullable
               )
               yield return column;
         }
      }

      public static IEnumerable<ColumnMap> GetRequiredStringColumns(this TableMap table)
      {
         foreach (var column in table.GetRequiredColumns())
         {
            if (column.Detail.SqlType.ToCsString() == "string")
               yield return column;
         }
      }

      public static string ScriptAsDefaultValue(this ColumnMap column, ScriptingOptions options)
      {
         return column.Detail.SqlType
            .ToScriptedDefaultValue(options, column.Detail);
      }
   }
}
