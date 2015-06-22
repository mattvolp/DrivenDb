using System;

namespace DrivenDb.Data
{
   internal class ColumnMap
   {      
      public ColumnMap(ColumnDetail detail, string customType)
      {
         Detail = detail;
         CustomType = customType;
      }

      public readonly ColumnDetail Detail;
      public readonly string CustomType;

      public bool HasCustomType
      {
         get { return !String.IsNullOrWhiteSpace(CustomType); }
      }
   }
}
