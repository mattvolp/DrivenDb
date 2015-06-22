namespace DrivenDb.Core
{
   public class DbEntityChange
   {
      public DbEntityChange(string columnName, object value)
      {
         ColumnName = columnName;
         Value = value;
      }

      public readonly string ColumnName;
      public readonly object Value;
   }
}