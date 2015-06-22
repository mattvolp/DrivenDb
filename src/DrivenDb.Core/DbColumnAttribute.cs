using System;

namespace DrivenDb.Core
{
   [AttributeUsage(AttributeTargets.Field)]
   public class DbColumnAttribute : Attribute
   {
      public bool IsGenerated
      {
         get;
         set;
      }

      public bool IsPrimary
      {
         get;
         set;
      }

      public string Name
      {
         get;
         set;
      }

      //public string Type
      //{
      //   get;
      //   set;
      //}
      
      //public bool IsNullable
      //{
      //   get;
      //   set;
      //}
   }
}
