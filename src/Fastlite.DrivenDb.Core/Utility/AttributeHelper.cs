using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Core.Utility
{
   internal static class AttributeHelper
   {
      public static DbTableAttribute GetTableAttribute(MemberInfo type)
      {
         return type.GetCustomAttributes(true)
            .OfType<DbTableAttribute>()
            .Select(a =>
            {
               a.Name = a.Name ?? type.Name;
               return a;
            })
            .Single();
      }

      public static DbSequenceAttribute GetSequenceAttribute(MemberInfo type)
      {
         return type.GetCustomAttributes(true)
            .OfType<DbSequenceAttribute>()
            .SingleOrDefault();
      }

      public static IDictionary<string, DbColumnAttribute> GetColumnAttributes(Type type)
      {
         var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
         var columns = new Dictionary<string, DbColumnAttribute>();

         foreach (var property in properties)
         {
            if (property.Name == "Record" || property.Name == "Entity")
            {
               continue;
            }

            var found = property.GetCustomAttributes(true)
               .OfType<DbColumnAttribute>()
               .FirstOrDefault();

            if (found != null)
            {
               found.Name = found.Name ?? property.Name;

               columns.Add(property.Name, found);
            }
         }

         return columns;
      }
   }
}
