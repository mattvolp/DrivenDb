using System.Data;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql.Interfaces
{
   internal interface IMsSqlScripter : IDbScripter
   {
      void ScriptInsertWithScopeIdentity<T>(IDbCommand command, T entity, int index, bool returnId)
         where T : IDbEntity, new();

      void ScriptUpdateOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new();

      void ScriptDeleteOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new();
   }
}