using System.Data;

namespace DrivenDb.MsSql
{
   internal interface IMsSqlScripter : IDbScripter
   {
      void ScriptInsertWithScopeIdentity<T>(IDbCommand command, T entity, int index, bool returnId)
         where T : IDbEntity, new();

      //void ScriptInsertOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
      //   where T : IDbEntity, new();

      void ScriptUpdateOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new();

      void ScriptDeleteOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new();
   }
}