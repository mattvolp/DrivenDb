using System;
using System.Text.RegularExpressions;

namespace Fastlite.DrivenDb.Data.Utility
{
   internal sealed class QueryIdentity : IEquatable<QueryIdentity>
   {
      private static readonly Regex _select = new Regex(@"(select).*?(from)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
      private readonly int _hashCode;
      private readonly string _sql;
      private readonly Type _type;

      internal QueryIdentity(string sql, Type type)
      {
         _sql = GetQuerySelects(sql);
         _type = type;

         unchecked
         {
            _hashCode = 17;
            _hashCode = _hashCode * 23 + (_sql == null ? 0 : _sql.GetHashCode());
            _hashCode = _hashCode * 23 + (type == null ? 0 : type.GetHashCode());
         }
      }

      public bool Equals(QueryIdentity other)
      {
         return other != null
                && _type == other._type
                && _sql == other._sql;
      }

      public override bool Equals(object obj)
      {
         return Equals(obj as QueryIdentity);
      }

      public override int GetHashCode()
      {
         return _hashCode;
      }

      private static string GetQuerySelects(string query)
      {
         var result = "";

         foreach (Match match in _select.Matches(query))
         {
            result += match.Value + Environment.NewLine;
         }

         return result;
      }
   }
}