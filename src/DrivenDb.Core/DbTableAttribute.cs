using System;

namespace DrivenDb.Core
{
   [AttributeUsage(AttributeTargets.Class)]
   public class DbTableAttribute : Attribute
   {
      public string Schema
      {
         get;
         set;
      }

      public string Name
      {
         get;
         set;
      }
   }
}
