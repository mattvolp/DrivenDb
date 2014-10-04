namespace DrivenDb.MsSql
{
   internal interface IMsSqlBuilder : ISqlBuilder
   {
      string ToInsertWithScopeIdentity(int index, bool returnId);

      //string ToInsertOutputDeleted<T>(T entity, int index, string[] columns)
      //   where T : IDbEntity;

      string ToUpdateOutputDeleted(int index, string[] columns);
      string ToDeleteOutputDeleted(int index, string[] columns);
   }
}