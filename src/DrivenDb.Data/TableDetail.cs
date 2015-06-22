
namespace DrivenDb.Data
{
   internal class TableDetail
   {
      public TableDetail(
           string schema
         , string name
         , ColumnDetail[] columns
         )
      {
         Schema = schema;
         Name = name;
         Columns = columns;
      }

      public readonly string Schema;
      public readonly string Name;
      public readonly ColumnDetail[] Columns;
   }
}
