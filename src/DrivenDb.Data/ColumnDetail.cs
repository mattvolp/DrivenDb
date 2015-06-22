namespace DrivenDb.Data
{
   internal class ColumnDetail
   {
      public ColumnDetail(
           DbType sqlType
         , string name
         , bool isNullable
         , bool isPrimary
         , bool isGenerated
         , bool isReadOnly
         , bool hasDefault
         , string defaultValue
         , int columnPosition
         )
      {
         SqlType = sqlType;
         Name = name;
         IsNullable = isNullable;
         IsPrimary = isPrimary;
         IsGenerated = isGenerated;
         IsReadOnly = isReadOnly;
         HasDefault = hasDefault;
         DefaultValue = defaultValue;
         ColumnPosition = columnPosition;
      }

      public readonly DbType SqlType;
      public readonly string Name;
      public readonly bool IsNullable;
      public readonly bool IsPrimary;
      public readonly bool IsGenerated;
      public readonly bool IsReadOnly;
      public readonly bool HasDefault;
      public readonly string DefaultValue;
      public readonly int ColumnPosition;
   }
}