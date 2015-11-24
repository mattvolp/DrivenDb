using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DrivenDb.Scripting.Internal
{
   internal static class Extensions
   {      
      private static readonly Regex ArgsEx = new Regex(@"\$(\d)");

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

      public static string Format(this string format, params object[] args)
      {
         return String.Format(format, args);
      }

      public static string Join<T>(this IEnumerable<T> items, string delimiter)
      {
         return string.Join(delimiter, items);
      }
   }
}
